using System;
using System.Collections.Generic;
using System.Text;

namespace JonkerBudgetCore.Api.Domain.Models.Categories
{
    public class CategoryCreateModel
    {
        public Guid GuidId { get; set; }
        public string CategoryName { get; set; }
        public double Budget { get; set; }
    }
}
