using JonkerBudgetCore.Api.Auth.Jwt;
using JonkerBudgetCore.Api.Auth.Providers;
using JonkerBudgetCore.Api.Auth.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Jwt")]
    public class JwtController : Controller
    {
        private readonly JwtIssuerOptions jwtOptions;
        private readonly ILogger logger;
        private readonly JsonSerializerSettings serializerSettings;
        private readonly IJwtIssuer jwtIssuer;
        private readonly IUserClaimsProvider userClaimsProvider;

        public JwtController(IOptions<JwtIssuerOptions> jwtOptions,
            IUserClaimsProvider userClaimsProvider,
            IJwtIssuer jwtIssuer,
            ILoggerFactory loggerFactory)
        {
            this.jwtIssuer = jwtIssuer;
            this.jwtOptions = jwtOptions.Value;
            this.jwtIssuer.ThrowIfInvalidOptions(this.jwtOptions);
            this.userClaimsProvider = userClaimsProvider;

            logger = loggerFactory.CreateLogger<JwtController>();

            serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromForm]ApplicationUser applicationUser)
        {
            var validationResult = await userClaimsProvider.GetClaimsIdentity(applicationUser);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ValidationError);
            }
            if (!validationResult.Identity.IsAuthenticated)
            {
                logger.LogInformation($"User ({applicationUser.UserName}) could not be Authenticated.");
                return BadRequest("Unable to Authorise Access. The User Account has not been Authenticated.");
            }
            var encodedJwt = jwtIssuer.IssueToken(applicationUser, jwtOptions, validationResult.Identity);

            // Serialize and return the response
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds
            };

            var json = JsonConvert.SerializeObject(response, serializerSettings);
            return new OkObjectResult(json);
        }
    }
}