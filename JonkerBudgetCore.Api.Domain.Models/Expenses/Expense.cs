using JonkerBudgetCore.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JonkerBudgetCore.Api.Domain.Models.Expenses
{
    public class Expense : Entity
    {
        public Expense() { }
        public Expense(string user)
            : base(user) { }

        public int Id { get; set; }
        public Guid CategoryGuidId { get; set; }
        public double ExpenseValue { get; set; }
        public int Year { get; set; }
        public string Month { get; set; }
        public DateTime? RecordDate { get; set; }
        public string ExpenseCode { get; set; }
        public string Comment { get; set; }
    }
}
