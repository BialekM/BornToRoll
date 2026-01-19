using System.Net;
using System.Net.Mail;

namespace BornToRollWebApi.Services.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"]!);
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"];
                var frontendUrl = _configuration["Email:FrontendUrl"];

                var resetLink = $"{frontendUrl}/reset-password?token={resetToken}";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail!, fromName),
                    Subject = "Password Reset Request - Born To Roll",
                    Body = $@"
                        <html>
                        <body>
                            <h2>Password Reset Request</h2>
                            <p>Hi,</p>
                            <p>You requested to reset your password. Click the link below to reset it:</p>
                            <p><a href='{resetLink}'>Reset Password</a></p>
                            <p>This link will expire in 1 hour.</p>
                            <p>If you didn't request this, please ignore this email.</p>
                            <br>
                            <p>Best regards,<br>Born To Roll Team</p>
                        </body>
                        </html>
                    ",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPassword),
                    EnableSsl = true
                };

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Password reset email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", toEmail);
                throw;
            }
        }
    }
}
