using JonkerBudgetCore.Api.Auth.Encrypt;
using JonkerBudgetCore.Api.Auth.Providers;
using JonkerBudgetCore.Api.Domain.Models.UserRoles;
using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Domain.Policies;
using JonkerBudgetCore.Api.Domain.Policy;
using JonkerBudgetCore.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JonkerBudgetCore.Api.Api.ViewModels.Users;
using JonkerBudgetCore.Api.Domain.Models.UserLogins;
using JonkerBudgetCore.Api.Domain.Models.Password;
using JonkerBudgetCore.Api.Domain.Shared_Services;
using CommsApi.Client.Core.Email.Classes;
using Microsoft.Extensions.Configuration;
using JonkerBudgetCore.Api.Domain.Configuration;

namespace JonkerBudgetCore.Api.Domain.Services.Users
{
    public class UsersService : IUsersService
    {
        // This should be configurable via startup. (Didn't have time)
        private const int minutesToAddForLockout = 5;
        private const int noOfPermissableFailedAttempts = 5;

        private readonly ApplicationDbContext dbContext;
        private readonly IEncrypter encrypter;
        private readonly IUserInfoProvider userInfoProvider;
        private readonly IPolicy<RegisterUserModel> registerUserPolicy;
        private readonly IPolicy<RegisterDomainUserModel> registerDomainUserPolicy;
        private readonly IPolicy<UpdateUserModel> updateUserPolicy;
        private readonly IPolicy<PasswordResetRequestModel> requestPasswordResetPolicy;
        private readonly IPolicy<ResetPasswordModel> resetPasswordPolicy;
        private readonly IPolicy<PasswordModel> passwordPolicy;
        private readonly ICommsService commsService;
        private readonly IConfigurationRoot configurationRoot;
        private readonly WebsiteConfiguration websiteConfiguration;

        public UsersService(ApplicationDbContext dbContext,
        IEncrypter encrypter,
        IUserInfoProvider userInfoProvider,
        IPolicy<RegisterUserModel> registerUserPolicy,
        IPolicy<RegisterDomainUserModel> registerDomainUserPolicy,
        IPolicy<UpdateUserModel> updateUserPolicy,
        IPolicy<PasswordResetRequestModel> requestPasswordResetPolicy,
        IPolicy<ResetPasswordModel> resetPasswordPolicy,
        IPolicy<PasswordModel> passwordPolicy,
        ICommsService commsService,
        IConfigurationRoot configurationRoot)
        {
            this.dbContext = dbContext;
            this.encrypter = encrypter;
            this.userInfoProvider = userInfoProvider;
            this.registerUserPolicy = registerUserPolicy;
            this.registerDomainUserPolicy = registerDomainUserPolicy;
            this.updateUserPolicy = updateUserPolicy;
            this.requestPasswordResetPolicy = requestPasswordResetPolicy;
            this.passwordPolicy = passwordPolicy;
            this.resetPasswordPolicy = resetPasswordPolicy;
            this.commsService = commsService;
            this.configurationRoot = configurationRoot;
            this.websiteConfiguration = new WebsiteConfiguration();

            configurationRoot.GetSection("WebsiteConfiguration").Bind(websiteConfiguration);
        }

        public async Task<User> RegisterUser(RegisterUserModel model)
        {
            if (!await registerUserPolicy.IsValid(model))
            {
                throw new PolicyViolationException(registerUserPolicy.PolicyViolations);
            }

            model.Password = "P@ssw0rd!";
            var salt = encrypter.GenerateSalt();
            var hashedPassword = encrypter.GenerateHash(model.Password, salt);

            var user = dbContext.Users.Add(new User(userInfoProvider.Username)
            {
                CreatedBy = userInfoProvider.Username,
                LastModifiedBy = userInfoProvider.Username,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Username = model.Username,
                ResetPasswordOnNextLogin = true,
                PasswordHash = model.IsAdUser ? null : hashedPassword,
                Salt = salt,
                IsActiveDirectoryUser = model.IsAdUser,
                Email = model.Email
            });

            CreateRoles(model.Roles, user.Entity);

            await dbContext.SaveChangesAsync();

            return user.Entity;
        }

        public async Task<User> UpdateUser(UpdateUserModel model)
        {
            if (!await updateUserPolicy.IsValid(model))
            {
                throw new PolicyViolationException(updateUserPolicy.PolicyViolations);
            }

            var userToUpdate = dbContext.Users.Include("UserRoles").First(match => match.UserId == model.UserId);
            userToUpdate.LastModifiedBy = userInfoProvider.Username;
            userToUpdate.LastModifiedDateUtc = DateTime.UtcNow;
            userToUpdate.Firstname = model.Firstname;
            userToUpdate.Lastname = model.Lastname;
            userToUpdate.Username = model.Username;
            userToUpdate.ResetPasswordOnNextLogin = true;
            userToUpdate.IsActiveDirectoryUser = model.IsAdUser;
            userToUpdate.Email = model.Email;

            UpdateRoles(model.Roles, userToUpdate);

            await dbContext.SaveChangesAsync();

            return userToUpdate;
        }

        private void UpdateRoles(int[] roles, User userToUpdate)
        {
            var rolesToAdd = roles.Except(userToUpdate.UserRoles.Select(m => m.RoleId));
            var rolesToDelete = userToUpdate.UserRoles.Where(item => !roles.Contains(item.RoleId));


            foreach (var roleToAdd in rolesToAdd)
            {
                dbContext.UserRoles.Add(new UserRole(userInfoProvider.Username)
                {
                    Role = dbContext.Roles.First(r => r.Id == roleToAdd),
                    User = userToUpdate
                });
            }

            dbContext.UserRoles.RemoveRange(rolesToDelete);
        }

        public async Task<User> RegisterDomainUser(RegisterDomainUserModel model)
        {
            if (!await registerDomainUserPolicy.IsValid(model))
            {
                throw new PolicyViolationException(registerDomainUserPolicy.PolicyViolations);
            }

            var user = dbContext.Users.Add(new User(userInfoProvider.Username)
            {
                CreatedBy = userInfoProvider.Username,
                LastModifiedBy = userInfoProvider.Username,
                Username = model.Username,
                IsActiveDirectoryUser = true
            });

            CreateRoles(model.Roles, user.Entity);

            await dbContext.SaveChangesAsync();

            return user.Entity;
        }

        private void CreateRoles(int[] roles, User user)
        {
            if (roles == null) return;

            foreach (var roleId in roles)
            {
                dbContext.UserRoles.Add(new UserRole(userInfoProvider.Username)
                {
                    Role = dbContext.Roles.First(r => r.Id == roleId),
                    User = user
                });
            }
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await dbContext
                .Users
                .Include("UserRoles.Role")
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<bool> Exists(string username)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return false;
            else
                return true;
        }

        public async Task<bool> Exists(string username, Guid id)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username && u.UserId != id);

            if (user == null)
                return false;
            else
                return true;
        }

        public async Task<bool> Exists(Guid userId)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return false;
            else
                return true;
        }

        public async Task<User> GetUser(string username, string password)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return null;

            var hashedPassword = encrypter.GenerateHash(password, user.Salt);

            return await dbContext
                .Users
                .Include("UserRoles.Role")
                .FirstOrDefaultAsync(s => s.Username == username && s.PasswordHash == hashedPassword);
        }

        public async Task<User> GetUser(string username)
        {
            return await dbContext
                .Users
                .Include("UserRoles.Role")
                .FirstOrDefaultAsync(s => s.Username == username);
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await dbContext
                .Users
                .Include("UserRoles.Role")
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<IEnumerable<User>> LookupUsers(string query)
        {
            return await dbContext
                .Users
                .Where(c => c.Username.Contains(query))
                .Include("UserRoles.Role")
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserWithEnabledRolesViewModel>> GetUsersWithEnabledRoles()
        {
            var users = await dbContext.Users.Include("UserRoles.Role").OrderBy(u => u.Firstname).ToListAsync();
            var allRoles = await dbContext.Roles.ToListAsync();

            var result = new List<UserWithEnabledRolesViewModel>();

            foreach (var user in users)
            {
                var userWithRolesViewModel = new UserWithEnabledRolesViewModel()
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Firstname = user.Firstname,
                    IsActive = user.IsActive,
                    Surname = user.Lastname,
                    CreatedBy = user.CreatedBy,
                    CreatedDateUtc = user.CreatedDateUtc,
                    IsActiveDirectoryUser = user.IsActiveDirectoryUser,
                    Username = user.Username,
                    Roles = new List<EnabledRoleViewModel>()
                };

                foreach (var allRole in allRoles)
                {
                    var isEnabled = false;

                    if (user.UserRoles.Select(r => r.RoleId).Contains(allRole.Id))
                    {
                        isEnabled = true;
                    }

                    userWithRolesViewModel.Roles.Add(new EnabledRoleViewModel()
                    {
                        Id = allRole.Id,
                        Name = allRole.Name,
                        Description = allRole.Description,
                        IsEnabled = isEnabled
                    });
                }

                result.Add(userWithRolesViewModel);
            }

            return result;
        }

        public async Task<bool> RecordSuccessfulLogin(Guid id)
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(n => n.UserId == id);

            user.FailedLoginAttempts = 0;
            user.LockoutExpiryDate = null;

            dbContext.UserLogins.Add(new UserLogin()
            {
                DateTimeIn = DateTime.Now,
                UserId = user.UserId
            });

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<LockoutInfoModel> RecordInvalidCredentialsUsed(Guid id)
        {
            var lockoutInfo = new LockoutInfoModel();

            var user = await dbContext.Users
                .FirstOrDefaultAsync(n => n.UserId == id);

            if (!user.FailedLoginAttempts.HasValue)
                user.FailedLoginAttempts = 0;

            user.FailedLoginAttempts = user.FailedLoginAttempts.Value + 1;

            if (user.FailedLoginAttempts % noOfPermissableFailedAttempts == 0)
            {
                int noOfMinutesToAdd = (user.FailedLoginAttempts.Value / noOfPermissableFailedAttempts) * minutesToAddForLockout;
                DateTime lockoutExpiry = DateTime.Now.AddMinutes(noOfMinutesToAdd);

                user.LockoutExpiryDate = lockoutExpiry;

                lockoutInfo.LockoutExpiryDate = lockoutExpiry;
                lockoutInfo.FailedLoginAttempts = user.FailedLoginAttempts.Value;
                lockoutInfo.IsLockedOut = true;
                lockoutInfo.LoginAttemptsRemaining = 0;
            }
            else
            {
                lockoutInfo.FailedLoginAttempts = user.FailedLoginAttempts.Value;
                lockoutInfo.IsLockedOut = false;
                lockoutInfo.LoginAttemptsRemaining = noOfPermissableFailedAttempts - (user.FailedLoginAttempts.Value % noOfPermissableFailedAttempts);
            }

            await dbContext.SaveChangesAsync();

            return lockoutInfo;
        }

        public async Task<PasswordResetRequestResponseModel> RequestPasswordReset(PasswordResetRequestModel model)
        {
            if (!await requestPasswordResetPolicy.IsValid(model))
            {
                throw new PolicyViolationException(requestPasswordResetPolicy.PolicyViolations);
            }

            var user = await dbContext
                .Users
                .Where(n => n.Username.Trim().ToUpper() == model.Username.Trim().ToUpper())
                .FirstAsync();

            //Expire Old Password Reset Requests
            var existingRequests = await dbContext.PasswordResetRequests.Where(n => n.UserId == user.UserId && n.IsActive).ToListAsync();

            foreach(var existingRequest in existingRequests)
            {
                existingRequest.IsActive = false;
            }

            //Generate New Request
            var resetRequest = new PasswordResetRequest(user.UserId);

            dbContext.PasswordResetRequests.Add(resetRequest);
            await dbContext.SaveChangesAsync();

            var email = this.commsService.GetBaseEmail();

            email.Subject = "JonkerBudgetCore.Api - Password Reset";
            email.IsBodyHtml = true;
            email.Body = $@"<style type=""text / css"">
                    p {{
                        font-family: ""sans-serif"";
                        font-size: 15px;
	                }}
                </style>
                <p>
                    Hi {user.Firstname},<br />
                    <br />
                    A password reset has been requested for your account ({user.Username}) on JonkerBudgetCore.Api ({websiteConfiguration.BaseAddress}).<br />
                    If you did not request a password reset or believe this is in error, please contact the administrator immediately.<br />
                    <br />
                    To reset your password, please click the following link:<br />
                    <a href=""{websiteConfiguration.BaseAddress}/reset-password/{resetRequest.UserId}/{resetRequest.Token}"">Reset My Password</a> <br />
                    <br />
                    Regards, <br />
                    JonkerBudgetCore.Api<br />                    
                </p>";
            email.To = user.Email;

            var emailResult = await this.commsService.SendEmail(email);
                        
            return new PasswordResetRequestResponseModel() { EmailAddress = user.Email };
        }

        public async Task<ResetPasswordResponseModel> ResetPassword(ResetPasswordModel model)
        {
            if (!await resetPasswordPolicy.IsValid(model))
            {
                throw new PolicyViolationException(resetPasswordPolicy.PolicyViolations);
            }

            var passwordModel = new PasswordModel()
            {
                Password = model.Password
            };

            if (!await passwordPolicy.IsValid(passwordModel))
            {
                throw new PolicyViolationException(passwordPolicy.PolicyViolations);
            }

            var user = await dbContext
                .Users
                .Where(n => n.UserId == model.UserId)
                .FirstAsync();

            var resetRequest = await dbContext
                .PasswordResetRequests
                .Where(n => n.UserId == user.UserId)
                .OrderByDescending(n => n.DateTimeIn)
                .FirstOrDefaultAsync();

            resetRequest.IsActive = false;
            resetRequest.PasswordResetTime = DateTime.Now;

            IEncrypter encrypter = new Encrypter();

            user.PasswordHash = encrypter.GenerateHash(model.Password, user.Salt);
            user.LockoutExpiryDate = null;
            user.FailedLoginAttempts = 0;
            user.ResetPasswordOnNextLogin = false;

            await dbContext.SaveChangesAsync();
            return new ResetPasswordResponseModel() { Username = user.Username };
        }
    }
}
