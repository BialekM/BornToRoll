using BornToRollWebApi.Data;
using BornToRollWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Add this using directive

namespace BornToRollWebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);
            if (exists)
                return BadRequest("User with this email already exists.");

            //ToDo: Hash password
            var passwordHash = request.Password;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHash
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { user.Id, user.Email });
        }
    }
}
