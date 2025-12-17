namespace Services;

public interface IEmailService
{
    Task<bool> SendMagicLinkAsync(string toEmail, string toName, string magicLink);
    Task<bool> SendOtpAsync(string toEmail, string toName, string otpCode);
    Task<bool> SendWelcomeEmailAsync(string toEmail, string toName, string magicLink);
    Task<bool> SendPasswordResetConfirmationAsync(string toEmail, string toName);
    Task<bool> SendOverdueReminderAsync(string toEmail, string toName, string bookTitle, DateTime dueDate);
}
