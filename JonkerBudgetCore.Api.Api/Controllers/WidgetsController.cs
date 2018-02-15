using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JonkerBudgetCore.Api.Domain.Services.Widgets;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using JonkerBudgetCore.Api.Api.ViewModels.Widgets;

namespace JonkerBudgetCore.Api.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Widgets")]
    public class WidgetsController : Controller
    {
        private IWidgetsService widgetService;
        private readonly IMapper mapper;


        public WidgetsController(IWidgetsService widgetService, IMapper mapper)
        {
            this.widgetService = widgetService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("WidgetData")]
        public async Task<IActionResult> GetWidgetData(int id)
        {
            var widgetQuery = await widgetService.GetWidgetQuery(id);

            if (widgetQuery.UseDemoDataProvider)
            {
                return Ok(await widgetService.GetWidgetDemoData(id));
            }
            else
            {
                return Ok(await widgetService.GetWidgetData(id));
            }
        }

        [HttpGet]
        [Route("WidgetDrillDownData")]
        public async Task<IActionResult> GetWidgetDrillDownData(int id)
        {
            var widgetQuery = await widgetService.GetWidgetQuery(id);

            if (widgetQuery.UseDemoDataProvider)
            {
                return Ok(new { Data = await widgetService.GetWidgetDemoData(id) });
            }
            else
            {
                return Ok(new { Data = await widgetService.GetWidgetData(id) });
            }
        }
    }
}