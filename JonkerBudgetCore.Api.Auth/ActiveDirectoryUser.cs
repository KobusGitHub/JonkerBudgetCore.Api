using System;
using System.Collections.Generic;
using System.Text;

namespace JonkerBudgetCore.Api.Auth
{
    public class ActiveDirectoryUser
    {
        public string UserName { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }
        public string Email { get; set; }
    }

}
