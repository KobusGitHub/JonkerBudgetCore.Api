using System;
using System.Collections.Generic;
using System.Text;

namespace JonkerBudgetCore.Api.Auth.ActiveDirectory
{
    public interface IActiveDirectoryProvider
    {
        IEnumerable<ActiveDirectoryUser> QueryActiveDirectory(string username);
    }
}
