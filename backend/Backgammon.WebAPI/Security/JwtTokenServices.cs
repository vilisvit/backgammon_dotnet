using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Backgammon.WebAPI.Security;

public class JwtTokenService(IConfiguration configuration)
{
    private readonly byte[] _key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
    private readonly string _issuer = configuration["Jwt:Issuer"]!;
    private readonly string _audience = configuration["Jwt:Audience"]!;
    private readonly int _expirationMs = int.Parse(configuration["Jwt:ExpiresInMs"] ?? "86400000"); // default 1 day

    public string GenerateToken(Guid userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(_key);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMilliseconds(_expirationMs),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
