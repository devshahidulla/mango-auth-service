using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mango.AuthService.Application.Interfaces;
using Mango.AuthService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Mango.AuthService.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
  private readonly IConfiguration _config;

  public JwtTokenService(IConfiguration config)
  {
    _config = config;
  }

  public string GenerateToken(User user)
  {
    var jwtSettings = _config.GetSection("JwtSettings");
    var key = jwtSettings["SecretKey"];
    var issuer = jwtSettings["Issuer"];
    var audience = jwtSettings["Audience"];
    var expires = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

    var claims = new[]
    {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("fullName", user.FullName)
        };

    var creds = new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        SecurityAlgorithms.HmacSha256
    );

    var token = new JwtSecurityToken(
        issuer, audience, claims,
        expires: DateTime.UtcNow.AddMinutes(expires),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
