using JonkerBudgetCore.Api.Domain.Models.Users;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Policy.Users
{
    public class PasswordPolicy : IPolicy<PasswordModel>
    {
        private List<PolicyViolation> policyViolations;

        public IEnumerable<PolicyViolation> PolicyViolations
        {
            get
            {
                return policyViolations;
            }
        }

        public async Task<bool> IsValid(PasswordModel model)
        {
            policyViolations = new List<PolicyViolation>();

            if (model.Password.Length < 6)
                policyViolations.Add(new PolicyViolation
                {
                    Key = "PasswordLength",
                    Value = "Password must be at least 6 characters long"
                });

            if (!model.Password.Any(c => char.IsUpper(c)))
                policyViolations.Add(new PolicyViolation
                {
                    Key = "PasswordComplexity",
                    Value = "Password must contain at least 1 capital letter"
                });

            if (!model.Password.Any(c => char.IsNumber(c)))
                policyViolations.Add(new PolicyViolation
                {
                    Key = "PasswordComplexity",
                    Value = "Password must contain at least 1 numeric"
                });

            await Task.FromResult(0);

            return !(policyViolations?.Count > 0);
        }
    }
}
