using Microsoft.AspNetCore.Mvc;
using Mango.AuthService.Application.DTOs;
using Mango.AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Mango.AuthService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
  {
    var response = await _authService.LoginAsync(request, cancellationToken);
    return Ok(response);
  }

  [Authorize]
  [HttpGet("me")]
  public IActionResult Me()
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var email = User.FindFirstValue(ClaimTypes.Email);

    return Ok(new
    {
      userId,
      email
    });
  }

}
