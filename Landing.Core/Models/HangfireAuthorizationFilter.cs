/*using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace Landing.Core.Models
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext;  

            // Проверка на наличие аутентификации
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false; // Не аутентифицирован, доступ закрыт
            }

            // Проверка на роль Admin
            if (httpContext.User.IsInRole("Admin"))
            {
                return true; // Доступ для администраторов
            }

            return false; // Не админ, доступ закрыт
        }
    }
}
*/