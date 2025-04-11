using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Landing.Core.Models
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity.IsAuthenticated &&
                   httpContext.User.IsInRole("Admin");
        }
    }
}
