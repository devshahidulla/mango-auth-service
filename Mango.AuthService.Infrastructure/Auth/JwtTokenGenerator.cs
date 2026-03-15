using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mango.AuthService.Application.Common.Auth;
using Mango.AuthService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Mango.AuthService.Infrastructure.Auth;

public class JwtTokenGenerator : IJwtTokenGenerator
{
  private readonly JwtSettings _jwtSettings;

  public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
  {
    _jwtSettings = jwtOptions.Value;
  }

  public string GenerateToken(User user)
  {
    var claims = new[]
    {
           new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
           new Claim(JwtRegisteredClaimNames.Email, user.Email),
           new Claim(ClaimTypes.Role, user.Role),
           new Claim("full_name", user.FullName ?? ""),
       };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
        audience: _jwtSettings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
