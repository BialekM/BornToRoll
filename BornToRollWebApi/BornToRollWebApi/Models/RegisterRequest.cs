namespace BornToRollWebApi.Models
{
    public class RegisterRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
