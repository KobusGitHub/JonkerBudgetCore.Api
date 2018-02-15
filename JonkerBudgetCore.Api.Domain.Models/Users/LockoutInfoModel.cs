using System;
using System.Collections.Generic;
using System.Text;

namespace JonkerBudgetCore.Api.Domain.Models.Users
{
    public class LockoutInfoModel
    {
        public bool IsLockedOut { get; set; }

        public int FailedLoginAttempts { get; set; }

        public int LoginAttemptsRemaining { get; set; }

        public DateTime? LockoutExpiryDate { get; set; }
    }
}
