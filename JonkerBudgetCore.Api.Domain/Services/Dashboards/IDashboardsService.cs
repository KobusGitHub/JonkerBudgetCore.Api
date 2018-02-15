using JonkerBudgetCore.Api.Domain.Models.Dashboards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Services.Dashboards
{
    public interface IDashboardsService
    {
        Task<IEnumerable<Dashboard>> GetMyDashboardsAsync();
    }
}
