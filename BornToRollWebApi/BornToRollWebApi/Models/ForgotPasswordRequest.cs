using System.ComponentModel.DataAnnotations;

namespace BornToRollWebApi.Models
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
