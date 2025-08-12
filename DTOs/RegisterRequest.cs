using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [StringLength(50, MinimumLength = 6)]
    public string UserName { get; set; }  // Email

    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    [Required]
    public string CompanyName { get; set; }

    [Required]
    public string ContactPerson { get; set; }

    [Phone]
    public string Phone { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public List<string> RoleNames { get; set; }
}
