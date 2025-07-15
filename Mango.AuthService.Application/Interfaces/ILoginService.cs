using Mango.AuthService.Application.DTOs;
namespace Mango.AuthService.Application.Interfaces;

public interface ILoginService
{
  Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
