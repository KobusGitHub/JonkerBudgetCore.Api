using JonkerBudgetCore.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using JonkerBudgetCore.Api.Domain.Models.WidgetQueries;

namespace JonkerBudgetCore.Api.Domain.Services.Widgets
{
    public class WidgetsService : IWidgetsService
    {
        private readonly ApplicationDbContext dbContext;

        public WidgetsService(
            ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Dictionary<string, object>>> GetWidgetData(int id)
        {
            var widgetQuery = await dbContext.WidgetQueries.FirstOrDefaultAsync(widgetQ => widgetQ.Id == id);

            if (widgetQuery == null)
            {
                return new List<Dictionary<string, object>>();
            }

            return await Task.FromResult(dbContext.SqlService.ExecuteSqlQuery(widgetQuery.ConnectionString, widgetQuery.SqlQuery));
        }

        public async Task<JArray> GetWidgetDemoData(int id)
        {
            var demoData = (await dbContext.WidgetQueries.SingleAsync(w => w.Id == id)).DemoDataProvider;

            return JArray.Parse(demoData);
        }

        public async Task<WidgetQuery> GetWidgetQuery(int id)
        {
            return await dbContext.WidgetQueries.FirstOrDefaultAsync(widgetQ => widgetQ.Id == id);
        }
    }
}
