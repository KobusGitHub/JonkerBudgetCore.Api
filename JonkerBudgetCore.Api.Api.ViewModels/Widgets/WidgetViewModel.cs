using Newtonsoft.Json.Linq;

namespace JonkerBudgetCore.Api.Api.ViewModels.Widgets
{
    public class WidgetViewModel
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public int RefreshInterval { get; set; }
        public string Heading { get; set; }
        public string Size { get; set; }
        public JObject ChartConfig { get; set; }
        public int WidgetQueryId { get; set; }
        public int? WidgetQueryDrilldownId { get; set; }
    }
}
