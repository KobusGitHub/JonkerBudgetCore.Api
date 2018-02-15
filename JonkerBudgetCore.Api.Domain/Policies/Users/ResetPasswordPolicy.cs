using JonkerBudgetCore.Api.Domain.Models.Password;
using JonkerBudgetCore.Api.Domain.Policy;
using JonkerBudgetCore.Api.Domain.Repositories.Users;
using JonkerBudgetCore.Api.Domain.Services.Users;
using JonkerBudgetCore.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Policies.Users
{
    public class ResetPasswordPolicy : IPolicy<ResetPasswordModel>
    {
        private List<PolicyViolation> policyViolations;
        private readonly IUsersRepository userRepository;
        private readonly ApplicationDbContext dbContext;

        public ResetPasswordPolicy(ApplicationDbContext dbContext,
            IUsersRepository userRepository)
        {
            this.dbContext = dbContext;
            this.userRepository = userRepository;
        }

        public IEnumerable<PolicyViolation> PolicyViolations
        {
            get
            {
                return policyViolations;
            }
        }

        public async Task<bool> IsValid(ResetPasswordModel model)
        {
            policyViolations = new List<PolicyViolation>();

            if (!await userRepository.Exists(model.UserId))
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "UserDoesNotExist",
                    Value = "User does not exist"
                });

                return false;
            }

            var user = await userRepository.GetUserById(model.UserId);

            if (!user.IsActive)
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "UserAccountDisabled",
                    Value = "User Account has been disabled."
                });

                return false;
            }

            if (user.IsActiveDirectoryUser)
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "UserBelongsToDomain",
                    Value = "Domain account can only be managed via Windows"
                });

                return false;
            }

            var resetRequest = await dbContext
                .PasswordResetRequests
                .Where(n => n.UserId == user.UserId)
                .OrderByDescending(n => n.DateTimeIn)
                .FirstOrDefaultAsync();

            if (resetRequest == null)
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "ResetRequestNotFound",
                    Value = "User Account has not requested a Password Reset"
                });

                return false;
            }

            if (resetRequest.Token != model.Token)
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "InvalidToken",
                    Value = "Invalid Token for Password Reset"
                });

                resetRequest.IsActive = false;
                await dbContext.SaveChangesAsync();

                return false;
            }

            if (!resetRequest.IsActive || resetRequest.PasswordResetTime.HasValue)
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "TokenActivatedOrDisabled",
                    Value = "Password Reset Token no longer valid"
                });

                return false;
            }

            if (resetRequest.DateTimeIn < DateTime.Now.AddDays(-3))
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "TokenExpired",
                    Value = "Password Reset Token has expired"
                });

                return false;
            }

            return true;
        }
    }
}
