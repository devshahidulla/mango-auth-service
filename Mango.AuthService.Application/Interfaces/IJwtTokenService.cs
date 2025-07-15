using Mango.AuthService.Domain.Entities;

namespace Mango.AuthService.Application.Interfaces;

public interface IJwtTokenService
{
  string GenerateToken(User user);
}
