using System;

namespace JonkerBudgetCore.Api.Domain.Models.Users
{
    public class UserChainModel
    {
        public Guid UserId { get; set; }
        public int SequenceNo { get; set; }
        public string Username { get; set; }
    }
}

