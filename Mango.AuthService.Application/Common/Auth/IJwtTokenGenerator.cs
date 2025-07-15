using Mango.AuthService.Domain.Entities;

namespace Mango.AuthService.Application.Common.Auth;

public interface IJwtTokenGenerator
{
  string GenerateToken(User user);
}
