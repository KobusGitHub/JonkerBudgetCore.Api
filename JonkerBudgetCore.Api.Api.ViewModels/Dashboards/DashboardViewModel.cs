using JonkerBudgetCore.Api.Api.ViewModels.Widgets;
using System.Collections.Generic;

namespace JonkerBudgetCore.Api.Api.ViewModels.Dashboards
{
    public class DashboardViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public IEnumerable<WidgetViewModel> Widgets { get; set; }
    }
}
