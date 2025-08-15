
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AirCargoRatesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            // Get the logged-in username from JWT token
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User identity not found.");

            // Fetch user from DB
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
                return NotFound("User not found.");

            // Build response
            var response = new DashboardResponse
            {
                UserName = user.UserName,
                Email = user.Email,
                firstName = user.ContactPerson,
                Company = user.CompanyName ?? "Not specified",
                Status =  user.CheckVerification ? "Active" : "Not Active",
                IsVerified = user.CheckVerification,
                Roles = user.UserRoles.Select(r => r.Role.RoleName).ToList()
            };

            return Ok(response);
        }
    }
}
