namespace BornToRollWebApi.Services.Email
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetToken);
    }
}
