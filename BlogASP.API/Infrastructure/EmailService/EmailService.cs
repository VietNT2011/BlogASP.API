using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace BlogASP.API.Infrastructure.EmailService
{
    public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
    {
        private readonly ILogger _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            // Get SMTP configuration from appsettings.json
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]);
            var smtpUser = _configuration["Smtp:User"];
            var smtpPass = _configuration["Smtp:Pass"];
            var fromEmail = _configuration["Smtp:From"];
            var fromName = _configuration["Smtp:Name"];

            //ArgumentNullException.ThrowIfNullOrEmpty(smtpHost, nameof(smtpHost));
            //ArgumentNullException.ThrowIfNullOrEmpty(smtpUser, nameof(smtpUser));
            //ArgumentNullException.ThrowIfNullOrEmpty(smtpPass, nameof(smtpPass));

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(fromName, fromEmail));
            email.To.Add(new MailboxAddress(null, toEmail));
            email.Subject = subject;

            // Set plain text and HTML content
            email.Body = new TextPart("html") { Text = message };

            try
            {
                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                await smtp.AuthenticateAsync(smtpUser, smtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email to {toEmail} sent successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {toEmail}: {ex.Message}");
                throw; // Rethrow the exception for debugging or higher-level handling
            }
        }
    }
}
