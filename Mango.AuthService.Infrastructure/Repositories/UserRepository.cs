using System.Data;
using Dapper;
using Mango.AuthService.Application.Interfaces;
using Mango.AuthService.Domain.Entities;

namespace Mango.AuthService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
  private readonly IDbConnection _dbConnection;

  public UserRepository(IDbConnection dbConnection)
  {
    _dbConnection = dbConnection;
  }

  public async Task<User?> GetByEmailAsync(string email)
  {
    const string sql = @"SELECT
  user_id AS ""UserId"",
  full_name AS ""FullName"",
  email AS ""Email"",
  password_hash AS ""PasswordHash"",
  role AS ""Role"",
  created_at AS ""CreatedAt""
FROM auth_users
WHERE email = @Email
 LIMIT 1";
    return await _dbConnection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
  }

  public async Task CreateAsync(User user)
  {
    const string sql = @"INSERT INTO auth_users (user_id, email, password_hash, role, full_name, created_at)
                         VALUES (@UserId, @Email, @PasswordHash, @Role, @FullName, @CreatedAt)";
    await _dbConnection.ExecuteAsync(sql, user);
  }

}
