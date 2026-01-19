using System.ComponentModel.DataAnnotations;

namespace BornToRollWebApi.Models
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = null!;
        
        [Required]
        [MinLength(2)]
        public string Name { get; set; } = null!;
    }
}
