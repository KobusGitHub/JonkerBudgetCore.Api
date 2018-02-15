using JonkerBudgetCore.Api.Auth.Providers;
using System;

namespace JonkerBudgetCore.Api.Tests.Integration.Mocks
{
    public class MockUserInfoProvider : IUserInfoProvider

    {
        public MockUserInfoProvider(string currentUser)
        {
            Username = currentUser;
        }

        public string Username { get; set; }
        public Guid UserId { get; set; }
    }
}
