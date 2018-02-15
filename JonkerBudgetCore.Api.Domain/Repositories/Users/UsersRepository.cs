using System.Threading.Tasks;
using JonkerBudgetCore.Api.Persistence;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using JonkerBudgetCore.Api.Domain.Models.Users;
using System;

namespace JonkerBudgetCore.Api.Domain.Repositories.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext dbContext;

        public UsersRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> Exists(string username, Guid userId)
        {
            var user = await dbContext
                .Users
                .FirstOrDefaultAsync(s => s.Username.ToUpper() == username.ToUpper() 
                    && s.UserId == userId);

            return user == null ? false : true;
        }

        public async Task<bool> Exists(Guid userId)
        {
            var user = await dbContext
                    .Users
                    .FirstOrDefaultAsync(s => s.UserId == userId);

            return user == null ? false : true;
        }

        public async Task<bool> Exists(string username)
        {
            var user = await dbContext
               .Users
               .FirstOrDefaultAsync(s => s.Username.ToUpper() == username.ToUpper());

            return user == null ? false : true;
        }

        public async Task<User> GetUserById(Guid userId)
        {
            var user = await dbContext
                                .Users
                                .FirstOrDefaultAsync(u => u.UserId == userId);

            return user;
        }

        public async Task<User> GetUserForUsername(string username)
        {
            var user = await dbContext
                                .Users
                                .FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }

        
    }
}
