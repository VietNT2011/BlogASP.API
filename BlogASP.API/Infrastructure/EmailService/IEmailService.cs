namespace BlogASP.API.Infrastructure.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
