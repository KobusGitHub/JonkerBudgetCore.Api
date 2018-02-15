using System.Collections.Generic;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Policy
{
    public interface IPolicy<T> where T : class
    {
        IEnumerable<PolicyViolation> PolicyViolations { get; }
        Task<bool> IsValid(T model);
    }
}
