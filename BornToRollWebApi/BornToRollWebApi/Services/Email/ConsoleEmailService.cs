using Microsoft.Extensions.Logging;

namespace BornToRollWebApi.Services.Email
{
    public class ConsoleEmailService : IEmailService
    {
        private readonly ILogger<ConsoleEmailService> _logger;
        private readonly IConfiguration _configuration;

        public ConsoleEmailService(ILogger<ConsoleEmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            var frontendUrl = _configuration["Email:FrontendUrl"];
            var resetLink = $"{frontendUrl}/reset-password?token={resetToken}";

            _logger.LogInformation("===== PASSWORD RESET EMAIL =====");
            _logger.LogInformation("To: {Email}", toEmail);
            _logger.LogInformation("Subject: Password Reset Request - Born To Roll");
            _logger.LogInformation("Reset Link: {ResetLink}", resetLink);
            _logger.LogInformation("Token: {Token}", resetToken);
            _logger.LogInformation("================================");

            return Task.CompletedTask;
        }
    }
}
