using Dapper;
using Ref.Core.Data;
using Ref.Core.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ref.Core.Repositories
{
    public interface IUserRepository
    {
        Task<int> CreateAsync(User user);
        Task<IQueryable<User>> FindByAsync(Expression<Func<User, bool>> predicate);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDbAccess _dbAccess;

        public UserRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<int> CreateAsync(User user)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"INSERT INTO Users (Email, Role, RegisteredAt, Guid, IsActive, Subscription) VALUES(@Email, @Role, @RegisteredAt, @Guid, @IsActive, @Subscription);
                    SELECT CAST(SCOPE_IDENTITY() as int)",
                    new
                    {
                        Email = user.Email.ToLowerInvariant(),
                        Role = "User",
                        RegisteredAt = DateTime.Now,
                        user.Guid,
                        IsActive = true,
                        Subscription = 3 //Demo
                    });
            }
        }

        public async Task<IQueryable<User>> FindByAsync(Expression<Func<User, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<User>(
                    @"SELECT Id, Email, PasswordHash, PasswordSalt, Role, Guid, RegisteredAt, IsActive FROM Users")).AsQueryable();

                return result.Where(predicate);
            }
        }
    }
}