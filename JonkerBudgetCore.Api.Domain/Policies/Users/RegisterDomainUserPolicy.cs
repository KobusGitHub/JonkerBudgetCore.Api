using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Domain.Policy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Policies.Users
{
    public class RegisterDomainUserPolicy : IPolicy<RegisterDomainUserModel>
    {
        private List<PolicyViolation> policyViolations;
        private IPolicy<UsernameModel> usernamePolicy;

        public RegisterDomainUserPolicy(IPolicy<UsernameModel> usernamePolicy)
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

        public async Task<bool> IsValid(RegisterDomainUserModel model)
        {
            policyViolations = new List<PolicyViolation>();

            if (!await usernamePolicy.IsValid(new UsernameModel { Username = model.Username }))
            {
                policyViolations.AddRange(usernamePolicy.PolicyViolations);
            }

            return !(policyViolations?.Count > 0);
        }
    }
}
