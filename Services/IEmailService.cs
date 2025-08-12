// Services/IEmailService.cs
public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otp);
}
