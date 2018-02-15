using JonkerBudgetCore.Api.Domain.Models.Dashboards;
using JonkerBudgetCore.Api.Domain.Models.WidgetQueries;
using JonkerBudgetCore.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace JonkerBudgetCore.Api.Domain.Models.Widgets
{
    public class Widget : Entity
    {
        public int Id { get; set; }
        public int RefreshInterval { get; set; }

        [StringLength(50)]
        public string Heading { get; set; }

        [StringLength(20)]
        public string Size { get; set; }

        public int Sequence { get; set; }

        public string ChartConfig { get; set; }

        public int DashboardId { get; set; }
        public Dashboard Dashboard { get; set; }

        public int WidgetQueryId { get; set; }
        public WidgetQuery WidgetQuery { get; set; }

        public int? WidgetQueryDrilldownId { get; set; }
        public WidgetQuery WidgetQueryDrilldown { get; set; }
    }
}
