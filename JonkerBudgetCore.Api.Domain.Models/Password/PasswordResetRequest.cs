using JonkerBudgetCore.Api.Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace JonkerBudgetCore.Api.Domain.Models.Password
{
    public class PasswordResetRequest
    {
        public PasswordResetRequest()
        {

        }

        public PasswordResetRequest(Guid userId)
        {
            this.Token = Guid.NewGuid();
            this.DateTimeIn = DateTime.Now;
            this.IsActive = true;
            this.UserId = userId;
        }

        public int Id { get; set; }

        public Guid Token { get; set; }

        public DateTime DateTimeIn { get; set; }        

        public DateTime? PasswordResetTime { get; set; }

        public bool IsActive { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }
    }
}
