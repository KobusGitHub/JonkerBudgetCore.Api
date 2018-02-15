using JonkerBudgetCore.Api.Api.ViewModels.Users;
using JonkerBudgetCore.Api.Domain.Models.Password;
using JonkerBudgetCore.Api.Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Services.Users
{
    public interface IUsersService
    {
        Task<User> RegisterUser(RegisterUserModel model);
        Task<User> RegisterDomainUser(RegisterDomainUserModel model);
        Task<User> UpdateUser(UpdateUserModel model);
        Task<bool> Exists(string username, Guid id);
        Task<bool> Exists(string username);
        Task<bool> Exists(Guid userId);
        Task<User> GetUser(Guid userId);
        Task<User> GetUser(string username, string password);
        Task<User> GetUser(string userName);
        Task<IEnumerable<User>> GetUsers();
        Task<IEnumerable<UserWithEnabledRolesViewModel>> GetUsersWithEnabledRoles();
        Task<IEnumerable<User>> LookupUsers(string query);
        Task<LockoutInfoModel> RecordInvalidCredentialsUsed(Guid id);
        Task<bool> RecordSuccessfulLogin(Guid id);
        Task<PasswordResetRequestResponseModel> RequestPasswordReset(PasswordResetRequestModel model);
        Task<ResetPasswordResponseModel> ResetPassword(ResetPasswordModel model);
    }
}
