using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using JonkerBudgetCore.Api.Api.ViewModels.Dashboards;
using JonkerBudgetCore.Api.Domain.Services.Dashboards;

namespace JonkerBudgetCore.Api.Api.Controllers
{
    [Route("api/Dashboards")]
    [Produces("application/json")]
    [Authorize]
    public class DashboardsController : Controller
    {
        private IDashboardsService dashboardsService;
        private readonly IMapper mapper;


        public DashboardsController(IDashboardsService dashboardsService, IMapper mapper)
        {
            this.dashboardsService = dashboardsService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("MyDashboards")]
        public async Task<IActionResult> GetMyDashboards()
        {
            var result = mapper.Map<IEnumerable<DashboardViewModel>>(await dashboardsService.GetMyDashboardsAsync());

            return Ok(result);
        }
    }
}