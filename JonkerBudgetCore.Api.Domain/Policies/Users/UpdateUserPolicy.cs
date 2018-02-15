using JonkerBudgetCore.Api.Auth.Providers;
using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Domain.Policy;
using JonkerBudgetCore.Api.Domain.Repositories.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Policies.Users
{
    public class UpdateUserPolicy : IPolicy<UpdateUserModel>
    {
        private List<PolicyViolation> policyViolations;
        private readonly IUserInfoProvider userInfoProvider;
        private readonly IUsersRepository usersRepository;        

        public UpdateUserPolicy(IUserInfoProvider userInfoProvider,            
            IUsersRepository usersRepository)
        {
            this.userInfoProvider = userInfoProvider;
            this.usersRepository = usersRepository;            
        }

        public IEnumerable<PolicyViolation> PolicyViolations
        {
            get
            {
                return policyViolations;
            }
        }

        public async Task<bool> IsValid(UpdateUserModel model)
        {
            policyViolations = new List<PolicyViolation>();

            var DuplicationUser = await usersRepository.Exists(model.Username, model.UserId);

            if (DuplicationUser)
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "InvalidData",
                    Value = "Duplication of Username."
                });               
            }

            var userExists = await usersRepository.Exists(model.UserId);
            if (!userExists)
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "InvalidData",
                    Value = "User does not exist."
                });

            }

            return !(policyViolations?.Count > 0);
        }
    }
}
