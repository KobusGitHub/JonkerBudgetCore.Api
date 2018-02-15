using JonkerBudgetCore.Api.Domain.Policy;
using System;
using System.Collections.Generic;

namespace JonkerBudgetCore.Api.Domain.Policies
{
    public class PolicyViolationException : Exception
    {
        private IEnumerable<PolicyViolation> violations;

        public PolicyViolationException(IEnumerable<PolicyViolation> violations)
        {
            this.violations = violations;
        }

        public IEnumerable<PolicyViolation> Violations
        {
            get
            {
                return violations;
            }
        }

        public PolicyViolationException(string message) : base(message)
        {
        }
    }
}
