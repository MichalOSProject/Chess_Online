using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Chess_Online.Server.Data;
using Chess_Online.Server.Data.Entity;
using System.Security.Claims;
using Chess_Online.Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public AuthService(IConfiguration confguration, ApplicationDbContext context)
    {
        _configuration = confguration;
        _context = context;
    }

    public async Task<string> GenerateTokenAsync(IdentityUser user)
    {
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationTime = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expirationTime,
            signingCredentials: creds);

        var newToken = new JwtSecurityTokenHandler().WriteToken(token);

        var jwtToken = new JwtTokens
        {
            Token = newToken,
            Jti = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value,
            UserId = user.Id,
            Expiration = expirationTime,
            Enabled = true
        };

        _context.JwtTokens.Add(jwtToken);
        _context.SaveChanges();

        return newToken;
    }
    public async Task<bool> ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        Console.WriteLine("token " + token);
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
    public async Task<JwtTokens> GetTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token)) return null;

        var jwtToken = await _context.JwtTokens.FirstOrDefaultAsync(t => t.Token == token);

        return jwtToken;
    }
}
