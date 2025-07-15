using Mango.AuthService.Application.Interfaces;
using Mango.AuthService.Domain.Entities;

namespace Mango.AuthService.Infrastructure.Services;

public class UserSyncService : IUserSyncService
{
  private readonly IUserRepository _userRepository;

  public UserSyncService(IUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task SaveUserAsync(User user, CancellationToken cancellationToken)
  {
    var existing = await _userRepository.GetByEmailAsync(user.Email);
    if (existing is null)
    {
      await _userRepository.CreateAsync(user);
    }
    // else you could update if needed
  }
}
