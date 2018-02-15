using JonkerBudgetCore.Api.Domain.Models.WidgetQueries;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Services.Widgets
{
    public interface IWidgetsService
    {
        Task<WidgetQuery> GetWidgetQuery(int id);
        Task<List<Dictionary<string, object>>> GetWidgetData(int id);
        Task<JArray> GetWidgetDemoData(int id);
    }
}
