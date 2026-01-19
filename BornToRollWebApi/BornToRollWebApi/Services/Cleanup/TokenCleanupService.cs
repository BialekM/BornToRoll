using BornToRollWebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BornToRollWebApi.Services.Cleanup
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanupService> _logger;

        public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredTokens();
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during token cleanup");
                }
            }
        }

        private async Task CleanupExpiredTokens()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var expiredTokens = await db.PasswordResetTokens
                .Where(t => t.ExpiresAt < DateTime.UtcNow || t.IsUsed)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                db.PasswordResetTokens.RemoveRange(expiredTokens);
                await db.SaveChangesAsync();
                _logger.LogInformation("Cleaned up {Count} expired password reset tokens", expiredTokens.Count);
            }
        }
    }
}
