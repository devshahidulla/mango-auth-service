using Mango.AuthService.Application.Interfaces;
using Mango.AuthService.Application.Common.Auth;
using Mango.AuthService.Application.DTOs;

namespace Mango.AuthService.Infrastructure.Services;

public class LoginService : ILoginService
{
  private readonly IUserRepository _userRepository;
  private readonly IJwtTokenGenerator _tokenGenerator;

  public LoginService(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator)
  {
    _userRepository = userRepository;
    _tokenGenerator = tokenGenerator;
  }

  public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByEmailAsync(request.Email);

    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    {
      throw new UnauthorizedAccessException("Invalid email or password.");
    }

    var token = _tokenGenerator.GenerateToken(user);
    return new LoginResponse
    {
      AccessToken = token,
      ExpiresIn = 3600
    };
  }
}
