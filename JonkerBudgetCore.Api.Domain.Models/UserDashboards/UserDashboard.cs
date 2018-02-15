using JonkerBudgetCore.Api.Domain.Models.Dashboards;
using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Entities;
using System;

namespace JonkerBudgetCore.Api.Domain.Models.UserDashboards
{
    public class UserDashboard : Entity
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public int DashboardId { get; set; }
        public Dashboard Dashboard { get; set; }
    }
}
