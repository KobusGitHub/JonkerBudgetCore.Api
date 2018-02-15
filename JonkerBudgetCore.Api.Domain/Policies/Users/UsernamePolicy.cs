using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Domain.Repositories.Users;
using JonkerBudgetCore.Api.Domain.Services.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Policy.Users
{
    public class UsernamePolicy : IPolicy<UsernameModel>
    {
        private List<PolicyViolation> policyViolations;
        private readonly IUsersRepository usersRepository;

        public UsernamePolicy(IUsersRepository usersRepository)
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

        public async Task<bool> IsValid(UsernameModel model)
        {
            policyViolations = new List<PolicyViolation>();

            if (await usersRepository.Exists(model.Username))
            {
                policyViolations.Add(new PolicyViolation
                {
                    Key = "UsernameInUse",
                    Value = "Username is already in use"
                });
            }

            return !(policyViolations?.Count > 0);
        }            
    }
}
