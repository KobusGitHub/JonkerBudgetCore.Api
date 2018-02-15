using JonkerBudgetCore.Api.Domain.Models.Password;
using JonkerBudgetCore.Api.Domain.Policy;
using JonkerBudgetCore.Api.Domain.Repositories.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Policies.Users
{
    public class RequestPasswordResetPolicy : IPolicy<PasswordResetRequestModel>
    {
        private List<PolicyViolation> policyViolations;
        private readonly IUsersRepository usersRepository;        

        public RequestPasswordResetPolicy(IUsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        public IEnumerable<PolicyViolation> PolicyViolations
        {
            get
            {
                return policyViolations;
            }
        }

        public async Task<bool> IsValid(PasswordResetRequestModel model)
        {
            policyViolations = new List<PolicyViolation>();

            if (!await usersRepository.Exists(model.Username))
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "UserDoesNotExist",
                    Value = "Username does not exist"
                });

                return false;
            }

            var user = await usersRepository.GetUserForUsername(model.Username);

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

            if (String.IsNullOrEmpty(user.Email))
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "EmailNotConfiguredForAccount",
                    Value = "User Account does not have an Email Address"
                });

                return false;
            }                        

            return true;
        }

    }
}
