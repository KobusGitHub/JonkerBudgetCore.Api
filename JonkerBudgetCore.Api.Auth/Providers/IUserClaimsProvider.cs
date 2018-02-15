using JonkerBudgetCore.Api.Auth.User;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Auth.Providers
{
    public interface IUserClaimsProvider
    {
        Task<ValidationResult> GetClaimsIdentity(ApplicationUser applicationUser);
    }
}
