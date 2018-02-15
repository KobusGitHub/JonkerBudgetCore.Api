using JonkerBudgetCore.Api.Auth.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Api.Middleware
{
    public static class CustomSerilogExtensions
    {
        public static IApplicationBuilder UseCustomSerilogProperties(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }

    public class LoggingMiddleware
    {       
        private readonly RequestDelegate _next;        
        private readonly IUserInfoProvider _userInfoProvider;

        public LoggingMiddleware(RequestDelegate next,            
            IUserInfoProvider userInfoProvider)
        {
            _next = next;            
            _userInfoProvider = userInfoProvider;       
        }

        public async Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("Username", _userInfoProvider.Username))
            {
                await _next.Invoke(context);
            }                
        }
    }
}
