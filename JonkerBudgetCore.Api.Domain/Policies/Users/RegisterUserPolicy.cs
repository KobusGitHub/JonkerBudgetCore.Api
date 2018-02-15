using System.Collections.Generic;
using JonkerBudgetCore.Api.Domain.Models.Users;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Policy.Users
{
    public class RegisterUserPolicy : IPolicy<RegisterUserModel>
    {
        private List<PolicyViolation> policyViolations;
        private IPolicy<UsernameModel> usernamePolicy;              

        public RegisterUserPolicy(IPolicy<UsernameModel> usernamePolicy)
        {          
            this.usernamePolicy = usernamePolicy;
        }

        public IEnumerable<PolicyViolation> PolicyViolations
        {
            get
            {
                return policyViolations;
            }
        }

        public async Task<bool> IsValid(RegisterUserModel model)
        {
            policyViolations = new List<PolicyViolation>();            

            if (! await usernamePolicy.IsValid(new UsernameModel { Username = model.Username }))
            {
                policyViolations.AddRange(usernamePolicy.PolicyViolations);
            }

            return !(policyViolations?.Count > 0);
        }
    }
}
