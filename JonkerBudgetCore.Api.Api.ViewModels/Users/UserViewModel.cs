using System;
using System.Collections.Generic;

namespace JonkerBudgetCore.Api.Api.ViewModels
{
    public class UserViewModel
    {
        public Guid UserId { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }

        public string Surname { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public string CreatedBy { get; set; }

        public List<int> Roles { get; set; }
        public bool IsActiveDirectoryUser { get; set; }
    }
}
