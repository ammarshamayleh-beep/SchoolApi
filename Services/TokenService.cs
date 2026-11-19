using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SchoolApi.Auth;
using SchoolApi.Models;
using SchoolApi.Options;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwt;

    // IOptions<JwtOptions>: the "Jwt" settings as a typed object, no magic strings
    public TokenService(IOptions<JwtOptions> options)
    {
        _jwt = options.Value;
    }

    public string CreateToken(User user)
    {
        // 1. The claims: what we write about the user inside the token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        // A teacher or a student also carries his id, so we know WHICH teacher or student
        if (user.TeacherId != null)
        {
            claims.Add(new Claim(AppClaims.TeacherId, user.TeacherId.Value.ToString()));
        }

        if (user.StudentId != null)
        {
            claims.Add(new Claim(AppClaims.StudentId, user.StudentId.Value.ToString()));
        }

        // 2. The secret key, to sign the token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

        // 3. Everything the token needs
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiresInMinutes),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        // 4. Build the token string: header.payload.signature
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
