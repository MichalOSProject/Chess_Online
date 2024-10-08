using Chess_Online.Server.Data.Entity;
using Microsoft.AspNetCore.Identity;


namespace Chess_Online.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
        Task<bool> ValidateToken(string token);
        Task<JwtTokens> GetTokenAsync(string token);
    }
}
