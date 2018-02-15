using System.Collections.Generic;
using System.Threading.Tasks;
using JonkerBudgetCore.Api.Domain.Models.Dashboards;
using JonkerBudgetCore.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using JonkerBudgetCore.Api.Auth.Providers;
using System.Linq;

namespace JonkerBudgetCore.Api.Domain.Services.Dashboards
{
    public class DashboardsService : IDashboardsService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IUserInfoProvider userInfoProvider;

        public DashboardsService(
            ApplicationDbContext dbContext,
            IUserInfoProvider userInfoProvider
        )
        {
            this.dbContext = dbContext;
            this.userInfoProvider = userInfoProvider;
        }

        public async Task<IEnumerable<Dashboard>> GetMyDashboardsAsync()
        {
            var username = userInfoProvider.Username;

            return await dbContext.Dashboards
               .Include("Widgets")
               .Include("UserDashboards.User")
               .Where(d => (d.UserDashboards.Any(ud => ud.User.Username == username)) || d.IsPublic)
               .ToListAsync();
        }
    }
}
