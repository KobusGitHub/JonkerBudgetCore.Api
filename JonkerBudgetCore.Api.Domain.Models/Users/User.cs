using JonkerBudgetCore.Api.Domain.Models.UserDashboards;
using JonkerBudgetCore.Api.Domain.Models.UserRoles;
using JonkerBudgetCore.Api.Domain.Models.UserLogins;
using JonkerBudgetCore.Api.Domain.Models.Password;
using JonkerBudgetCore.Api.Entities;
using System;
using System.Collections.Generic;

namespace JonkerBudgetCore.Api.Domain.Models.Users
{
    public class User : Entity
    {
        public User() { }
        public User(string user) : base(user) { }

        public Guid UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public bool ResetPasswordOnNextLogin {get;set;}
        public string Username { get; set; }
        public bool IsActiveDirectoryUser { get; set; }
        public byte[] Salt { get; set; }        
        public string PasswordHash { get; set; }
        public int? FailedLoginAttempts { get; set; }
        public DateTime? LockoutExpiryDate { get; set; }

        // Navigation Properties
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<UserDashboard> UserDashboards { get; set; }
        public ICollection<UserLogin> UserLogins { get; set; }
        public ICollection<PasswordResetRequest> PasswordResetRequests { get; set; }
    }
}
