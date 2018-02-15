using System;

namespace JonkerBudgetCore.Api.Api.ViewModels.Users
{
    public class EnabledRoleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public String Description { get; set; }
        public bool IsEnabled { get; set; }
    }
}
