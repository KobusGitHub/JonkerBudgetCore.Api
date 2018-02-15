using JonkerBudgetCore.Api.Domain.Models.Users;
using System.Threading.Tasks;
using System;

namespace JonkerBudgetCore.Api.Domain.Repositories.Users
{
    public interface IUsersRepository
    {
        Task<User> GetUserForUsername(string username);
        Task<User> GetUserById(Guid userId);
        Task<bool> Exists(string username, Guid userId);
        Task<bool> Exists(Guid userId);
        Task<bool> Exists(string username);        
    }
}
