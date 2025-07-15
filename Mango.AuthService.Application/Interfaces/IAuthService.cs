using Mango.AuthService.Application.DTOs;

namespace Mango.AuthService.Application.Interfaces;

public interface IAuthService
{
  Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
