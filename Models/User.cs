using System.ComponentModel.DataAnnotations;

public class User
{
    public int UserId { get; set; }

    [Required, EmailAddress]
    public string UserName { get; set; }  // Email ID used for login

    public string CompanyName { get; set; }
    public string ContactPerson { get; set; }

    [Phone]
    public string Phone { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; } // For storing hashed password


    public bool CheckVerification { get; set; } = false;
    public string? EmailOtp { get; set; }
    public DateTime? OtpGeneratedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }
}
