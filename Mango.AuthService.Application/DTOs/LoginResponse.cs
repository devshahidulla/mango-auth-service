namespace Mango.AuthService.Application.DTOs;

public class LoginResponse
{
  public string AccessToken { get; set; } = default!;
  public int ExpiresIn { get; set; }
}
