using JonkerBudgetCore.Api.Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace JonkerBudgetCore.Api.Domain.Models.UserLogins
{
    public class UserLogin
    {
        public int Id { get; set; }

        public DateTime DateTimeIn { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
