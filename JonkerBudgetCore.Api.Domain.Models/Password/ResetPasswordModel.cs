using System;
using System.Collections.Generic;
using System.Text;

namespace JonkerBudgetCore.Api.Domain.Models.Password
{
    public class ResetPasswordModel
    {
        public Guid UserId { get; set; }

        public Guid Token { get; set; }

        public string Password { get; set; }
    }
}
