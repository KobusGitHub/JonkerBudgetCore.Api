using JonkerBudgetCore.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace JonkerBudgetCore.Api.Domain.Models.WidgetQueries
{
    public class WidgetQuery : Entity
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public bool UseDemoDataProvider { get; set; }
        public string DemoDataProvider { get; set; }

        public string ConnectionString { get; set; }

        public string SqlQuery { get; set; }
    }
}
