using JonkerBudgetCore.Api.Domain.Services.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Roles")]
    public class RolesController : Controller
    {
        private readonly IRolesService rolesService;

        public RolesController(IRolesService rolesService)
        {
            this.rolesService = rolesService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, UserManagement")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await rolesService.GetRoles());
        }
    }
}