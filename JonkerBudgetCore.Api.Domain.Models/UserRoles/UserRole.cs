using JonkerBudgetCore.Api.Domain.Models.Roles;
using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Entities;
using System;

namespace JonkerBudgetCore.Api.Domain.Models.UserRoles
{
    public class UserRole : Entity
    {
        public UserRole() {}
        public UserRole(string user) 
            : base(user) {}
        
        public Guid UserId { get; set; }        
        public int RoleId { get; set; }

        // Navigation Properties
        public Role Role { get; set; }
        public User User { get; set; }
    }
}
