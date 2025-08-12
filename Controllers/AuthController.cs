using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IEmailService _emailService;

    public AuthController(AppDbContext context, JwtTokenGenerator tokenGenerator, IPasswordHasher<User> passwordHasher, IEmailService emailService)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    [HttpPost("login")]
    [AllowAnonymous] // This is crucial - it allows unauthenticated access
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == request.UserName);
        
        if (user == null)
            return Unauthorized("Invalid credentials");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid credentials");

        var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
        var token = _tokenGenerator.GenerateToken(user, roles);

        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
            return BadRequest("User already exists");

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            CompanyName = request.CompanyName,
            ContactPerson = request.ContactPerson,
            Phone = request.Phone,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        // Generate 6-digit OTP
        var otp = new Random().Next(100000, 999999).ToString();
        user.EmailOtp = otp;
        user.OtpGeneratedAt = DateTime.UtcNow;

        var allRoles = await _context.Roles.ToListAsync();
        var selectedRoles = allRoles
        .Where(r => request.RoleNames.Contains(r.RoleName))
        .ToList();

        user.UserRoles = selectedRoles.Select(role => new UserRole
        {
            Role = role,
            User = user
        }).ToList();

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var roles = selectedRoles.Select(r => r.RoleName).ToList();
        var token = _tokenGenerator.GenerateToken(user, roles);


        // TODO: Send OTP using SMTP or an email service
        await _emailService.SendOtpEmailAsync(user.Email, otp);

        return Ok(new AuthResponse
        {
            Token = token,
            UserName = user.UserName,
            Roles = roles
        });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmailOtp([FromBody] EmailOtpRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.Email);

        if (user == null)
            return NotFound("User not found");

        if (user.CheckVerification)
            return BadRequest("Email already verified");

        if (user.EmailOtp != request.Otp)
            return BadRequest("Invalid OTP");

        if (user.OtpGeneratedAt != null && user.OtpGeneratedAt.Value.AddMinutes(10) > DateTime.UtcNow)
        {
            //nothing to do            
        }
        else
        {
            return BadRequest("OTP expired");
        }

        user.CheckVerification = true;
        user.EmailOtp = null;
        user.OtpGeneratedAt = null;

        await _context.SaveChangesAsync();
        return Ok("Email verified successfully");
    }



}
