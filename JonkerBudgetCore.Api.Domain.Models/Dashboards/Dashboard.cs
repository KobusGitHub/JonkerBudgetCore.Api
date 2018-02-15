using JonkerBudgetCore.Api.Domain.Models.UserDashboards;
using JonkerBudgetCore.Api.Domain.Models.Widgets;
using JonkerBudgetCore.Api.Entities;
using System.Collections.Generic;

namespace JonkerBudgetCore.Api.Domain.Models.Dashboards
{
    public class Dashboard : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Icon { get; set; }
        public bool IsPublic { get; set; }

        public ICollection<Widget> Widgets { get; set; }
        public ICollection<UserDashboard> UserDashboards { get; set; }
    }
}
