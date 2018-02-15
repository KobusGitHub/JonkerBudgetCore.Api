using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Api.Providers
{
    public class ActiveDirectoryOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
