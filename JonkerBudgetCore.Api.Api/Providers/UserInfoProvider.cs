using JonkerBudgetCore.Api.Auth.Providers;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace JonkerBudgetCore.Api.Api.Providers
{
    public class UserInfoProvider : IUserInfoProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserInfoProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;            
        }

        public string Username
        {
            get
            {
                var username = "Anonymous";

                if (httpContextAccessor.HttpContext == null)
                    return username;

                if (httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)                    
                    username = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                
                return username;                
            }
        }

        public Guid UserId
        {
            get
            {
                var userId = Guid.Empty;

                if (httpContextAccessor.HttpContext != null)
                    if (httpContextAccessor.HttpContext.User.FindFirst("uid") != null)
                        userId = new Guid(httpContextAccessor.HttpContext.User.FindFirst("uid").Value);

                return userId;
            }
        }
    }
}
