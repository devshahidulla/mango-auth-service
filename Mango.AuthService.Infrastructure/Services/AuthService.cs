using System.Security.Cryptography;
using System.Text;
using Mango.AuthService.Application.DTOs;
using Mango.AuthService.Application.Interfaces;
using BCrypt.Net;
using System.Collections.Generic;
namespace Mango.AuthService.Infrastructure.Services;

public class AuthService : IAuthService
{
  private readonly IUserRepository _userRepository;
  private readonly IJwtTokenService _jwtTokenService;

  public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
  {
    _userRepository = userRepository;
    _jwtTokenService = jwtTokenService;
  }

  public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByEmailAsync(request.Email);

    if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
      throw new UnauthorizedAccessException("Invalid email or password.");

    var token = _jwtTokenService.GenerateToken(user);

    return new LoginResponse
    {
      AccessToken = token,
      ExpiresIn = 3600 // match JWT config
    };
  }

  private static bool VerifyPassword(string plainPassword, string hash)
  {
    return BCrypt.Net.BCrypt.Verify(plainPassword, hash);
  }
}
