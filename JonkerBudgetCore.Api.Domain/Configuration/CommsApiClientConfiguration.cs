using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Configuration
{
    public class CommsApiClientConfiguration
    {
        public string ApiAddress { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string BccRecipients { get; set; }

        public string FromAddress { get; set; }

        public string CustomerReference { get; set; }
    }
}
