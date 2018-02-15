using System;

namespace JonkerBudgetCore.Api.Auth.Providers
{
    public interface IUserInfoProvider
    {
        string Username { get; }
        Guid UserId { get; }
    }
}
