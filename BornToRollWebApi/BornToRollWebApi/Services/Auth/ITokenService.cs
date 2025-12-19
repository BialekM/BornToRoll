using BornToRollWebApi.Models;

namespace BornToRollWebApi.Services.Auth
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
