namespace Services;

public class GmailEmailService : IEmailService
{
    // TODO: Add SMTP configuration from appsettings.json
    // TODO: Add System.Net.Mail or MailKit for email sending

    public async Task<bool> SendMagicLinkAsync(string toEmail, string toName, string magicLink)
    {
        // TODO: Implement email sending with magic link
        var subject = "Your Magic Link to Login";
        var body = $"Hello {toName},\n\nClick the link below to login:\n{magicLink}\n\nThis link will expire in 24 hours.";
        
        // Placeholder: actual implementation will use SMTP
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> SendOtpAsync(string toEmail, string toName, string otpCode)
    {
        // TODO: Implement email sending with OTP
        var subject = "Your Password Reset Code";
        var body = $"Hello {toName},\n\nYour OTP code is: {otpCode}\n\nThis code will expire in 5 minutes.";
        
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string toName, string magicLink)
    {
        // TODO: Implement welcome email
        var subject = "Welcome to FU Library";
        var body = $"Hello {toName},\n\nWelcome! Click the link below to set up your account:\n{magicLink}";
        
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> SendPasswordResetConfirmationAsync(string toEmail, string toName)
    {
        // TODO: Implement password reset confirmation email
        var subject = "Password Reset Successful";
        var body = $"Hello {toName},\n\nYour password has been successfully reset.";
        
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> SendOverdueReminderAsync(string toEmail, string toName, string bookTitle, DateTime dueDate)
    {
        // TODO: Implement overdue reminder email
        var subject = "Book Return Reminder";
        var body = $"Hello {toName},\n\nReminder: '{bookTitle}' is due on {dueDate:yyyy-MM-dd}. Please return it on time.";
        
        await Task.CompletedTask;
        return true;
    }
}
