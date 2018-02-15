using System.Security.Claims;
using JonkerBudgetCore.Api.Auth.User;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Auth.Jwt
{
    public interface IJwtIssuer
    {
        void ThrowIfInvalidOptions(JwtIssuerOptions jwtOptions);                
        Task<string> IssueToken(ApplicationUser applicationUser, JwtIssuerOptions jwtOptions, ClaimsIdentity identity);
    }
}
