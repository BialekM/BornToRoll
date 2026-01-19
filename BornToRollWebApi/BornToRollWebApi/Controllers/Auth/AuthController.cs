using BornToRollWebApi.Data;
using BornToRollWebApi.Models;
using BornToRollWebApi.Services.Auth;
using BornToRollWebApi.Services.Email;
using BornToRollWebApi.Services.RateLimit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BornToRollWebApi.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly RateLimitService _rateLimitService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AppDbContext db, 
            IPasswordHasher passwordHasher, 
            ITokenService tokenService, 
            IEmailService emailService,
            RateLimitService rateLimitService,
            ILogger<AuthController> logger)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _emailService = emailService;
            _rateLimitService = rateLimitService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validation = PasswordValidator.Validate(request.Password);
            if (!validation.IsValid)
                return BadRequest(validation.ErrorMessage);

            var normalizedEmail = request.Email.ToLowerInvariant();
            var exists = await _db.Users.AnyAsync(u => u.Email == normalizedEmail);
            if (exists)
            {
                _logger.LogWarning("Registration attempt with existing email: {Email}", normalizedEmail);
                return BadRequest("User with this email already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = normalizedEmail,
                Name = request.Name,
                PasswordHash = _passwordHasher.Hash(request.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _logger.LogInformation("New user registered: {Email}", normalizedEmail);

            return Ok(new
            {
                Token = _tokenService.GenerateToken(user),
                user.Id,
                user.Email,
                user.Name
            });!ModelState.IsValid)
                return BadRequest(ModelState);

            var normalizedEmail = request.Email.ToLowerInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            
            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent email: {Email}", normalizedEmail);
                return Unauthorized("Invalid credentials.");
            }

            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", normalizedEmail);
                return Unauthorized("Invalid credentials.");
            }

            _logger.LogInformation("Successful login for email: {Email}", normalizedEmail(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized("Invalid credentials.");

            return Ok(new
            {
                Token = _tokenService.GenerateToken(user!),
                user.Id,
                user.Email,
                user.Name
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
!ModelState.IsValid)
                return BadRequest(ModelState);

            var normalizedEmail = request.Email.ToLowerInvariant();

            // Rate limiting
            if (_rateLimitService.IsRateLimited($"forgot-password:{normalizedEmail}"))
            {
                _logger.LogWarning("Rate limit exceeded for forgot-password: {Email}", normalizedEmail);
                return TooManyRequests("Too many password reset attempts. Please try again later.");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            
            // Always return OK to prevent email enumeration
            if (user == null)
            {
                _logger.LogInformation("Password reset requested for non-existent email: {Email}", normalizedEmail);
                return Ok(new { message = "If the email exists, a reset link has been sent." });
            }

            // Generate secure random token (URL-safe)
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(tokenBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");

            // Hash token for storage
            var tokenHash = HashToken(token);
            
            var resetToken = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                Token = tokenHash,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsUsed = false
            };

            _db.PasswordResetTokens.Add(resetToken);
            await _db.SaveChangesAsync();

            awai!ModelState.IsValid)
                return BadRequest(ModelState);

            var validation = PasswordValidator.Validate(request.NewPassword);
            if (!validation.IsValid)
                return BadRequest(validation.ErrorMessage);

            var tokenHash = HashToken(request.Token);

            var resetToken = await _db.PasswordResetTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == tokenHash);

            if (resetToken == null)
            {
                _logger.LogWarning("Password reset attempted with invalid token");
                return BadRequest("Invalid or expired token.");
            }

            if (resetToken.IsUsed)
            {
                _logger.LogWarning("Password reset attempted with already used token");
                return BadRequest("Invalid or expired token.");
            }

            if (resetToken.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Password reset attempted with expired token. Expired at: {ExpiresAt}", resetToken.ExpiresAt);
                return BadRequest("Invalid or expired token.");
            }

            resetToken.User.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            resetToken.IsUsed = true;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Password successfully reset for user: {Email}", resetToken.User.EmailPasswordResetTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.Token);

            if (resetToken == null)
            {
                Console.WriteLine("DEBUG: Token not found in database");
                return BadRequest("Invalid or expired token.");
            }

            if (resetToken.IsUsed)
            {
                Console.WriteLine("DEBUG: Token already used");
                return BadRequest("Invalid or expired token.");
            }

            if (resetToken.ExpiresAt < DateTime.UtcNow)
            {
                Console.WriteLine($"DEBUG: Token expired. ExpiresAt: {resetToken.ExpiresAt}, Now: {DateTime.UtcNow}");
                return BadRequest("Invalid or expired token.");
            }

            if (request.NewPassword.Length < 6)
                return BadRequest("Password must be at least 6 characters.");

            resetToken.User.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            resetToken.IsUsed = true;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Password has been reset successfully." });
        }
    }
}
