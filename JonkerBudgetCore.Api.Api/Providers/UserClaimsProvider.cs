using JonkerBudgetCore.Api.Auth.User;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using JonkerBudgetCore.Api.Domain.Models.Users;
using System.Linq;
using Novell.Directory.Ldap;
using Microsoft.Extensions.Logging;
using JonkerBudgetCore.Api.Auth.Providers;
using JonkerBudgetCore.Api.Domain.Services.Users;
using JonkerBudgetCore.Api.Auth;
using System;

namespace JonkerBudgetCore.Api.Api.Providers
{
    public class UserClaimsProvider : IUserClaimsProvider
    {
        private readonly IUsersService usersQueryService;
        private readonly ILogger logger;

        public UserClaimsProvider(IUsersService usersQueryService,
            ILogger<UserClaimsProvider> logger)
        {
            this.usersQueryService = usersQueryService;
            this.logger = logger;
        }

        public async Task<ValidationResult> GetClaimsIdentity(ApplicationUser user)
        {
            //Check if user is exists and type of user
            var dbUserWithRoles = await usersQueryService
                    .GetUser(user.UserName);


            if (dbUserWithRoles == null)
                return new ValidationResult("Account does not exist");

            if (!dbUserWithRoles.IsActive)
            {
                logger.LogError($"Attempted access to deactivated account {user.UserName}");
                return new ValidationResult("Account deactivated. Please contact the system administrator for more information.");
            }

            if (dbUserWithRoles.LockoutExpiryDate.HasValue && dbUserWithRoles.LockoutExpiryDate.Value > DateTime.Now)
            {
                logger.LogError($"Attempted access to locked account {user.UserName}");
                return GenerateLockedAccountError(dbUserWithRoles.Username, dbUserWithRoles.LockoutExpiryDate.Value);
            }

            if (dbUserWithRoles.IsActiveDirectoryUser)
            {
                try
                {
                    using (var cn = new LdapConnection())
                    {
                        cn.Connect("sv-ad1.supergrp.net", 389); //connect
                        cn.Bind(user.UserName, user.Password); // bind with credentials                        
                    }


                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex.ToString());
                    var result = await usersQueryService.RecordInvalidCredentialsUsed(dbUserWithRoles.UserId);

                    if (result.IsLockedOut)
                    {
                        logger.LogError($"Account Locked due to excessive failed login attempts - {user.UserName}");
                        return GenerateLockedAccountError(dbUserWithRoles.Username, result.LockoutExpiryDate.Value);
                    }
                    else
                    {
                        logger.LogError($"Account Login failed for {user.UserName}");
                        return new ValidationResult($"Invalid Password. { result.LoginAttemptsRemaining } login attempts remaining.");
                    }
                }

                await usersQueryService.RecordSuccessfulLogin(dbUserWithRoles.UserId);
                return new ValidationResult(GenerateIdentityWithClaims(user, dbUserWithRoles));
            }
            else
            {
                var userId = dbUserWithRoles.UserId;
                //Authenticate user with username and password
                dbUserWithRoles = await usersQueryService
                                            .GetUser(user.UserName, user.Password);

                //could not authenticate user
                if (dbUserWithRoles == null)
                {
                    var result = await usersQueryService.RecordInvalidCredentialsUsed(userId);

                    if (result.IsLockedOut)
                    {
                        logger.LogError($"Account Locked due to excessive failed login attempts - {user.UserName}");
                        return GenerateLockedAccountError(user.UserName, result.LockoutExpiryDate.Value);
                    }
                    else
                    {
                        logger.LogError($"Account Login failed for {user.UserName}");
                        return new ValidationResult($"Invalid Password. { result.LoginAttemptsRemaining } login attempts remaining.");
                    }
                }

                await usersQueryService.RecordSuccessfulLogin(dbUserWithRoles.UserId);
                return new ValidationResult(GenerateIdentityWithClaims(user, dbUserWithRoles));
            }
        }

        private ValidationResult GenerateLockedAccountError(string username, DateTime lockoutExpiryDate)
        {
            var timeUntilExpiry = "";
            var dateDiff = (lockoutExpiryDate - DateTime.Now);

            if (dateDiff.TotalMinutes < 1)
            {
                timeUntilExpiry = (int)dateDiff.TotalSeconds + " seconds";
            }
            else
            {
                timeUntilExpiry = (int)dateDiff.TotalMinutes + " minutes";
            }

            return new ValidationResult($"Account locked due to excessive failed login attempts. Please try again in {timeUntilExpiry}.");
        }

        private ClaimsIdentity GenerateIdentityWithClaims(ApplicationUser user, User dbUserWithRoles)
        {
            var identity = new ClaimsIdentity(
                            //Token
                            new GenericIdentity(user.UserName, "Token"),
                            //Custom
                            new[]
                            {
                                new Claim("uid", dbUserWithRoles.UserId.ToString()),
                                new Claim("surname", dbUserWithRoles.Lastname),
                                 new Claim("firstName", dbUserWithRoles.Firstname)
                            });

            //Roles
            identity = AddRoles(identity, dbUserWithRoles);
            return identity;
        }

        private ClaimsIdentity AddRoles(ClaimsIdentity identity, User user)
        {
            foreach (var role in user.UserRoles.Select(s => s.Role))
            {
                identity.AddClaim(new Claim("roles", role.Name));
            }

            return identity;
        }
        
    }
}
