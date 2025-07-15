using Mango.AuthService.Domain.Entities;

namespace Mango.AuthService.Application.Interfaces;

public interface IUserSyncService
{
  Task SaveUserAsync(User user, CancellationToken cancellationToken);
}
