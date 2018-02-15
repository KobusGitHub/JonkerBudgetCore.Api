using JonkerBudgetCore.Api.Domain.Policy;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace JonkerBudgetCore.Api.Api.Exceptions
{
    public class PolicyErrorModel
    {
        public int Code { get; set; }
        public IEnumerable<PolicyViolation> Violations { get; set; }

        // other fields

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
