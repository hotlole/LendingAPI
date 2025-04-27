using Hangfire.Dashboard;
using Landing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Landing.Core.Models
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var path = httpContext.Request.Path.Value;

            // Разрешаем грузить CSS, JS, шрифты без авторизации
            if (path != null && (
                path.StartsWith("/hangfire/css") ||
                path.StartsWith("/hangfire/js") ||
                path.StartsWith("/hangfire/fonts")
            ))
            {
                return true;
            }

            // Проверяем наличие Bearer токена
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Тут простая проверка — токен вообще передан
                return !string.IsNullOrEmpty(token);
            }

            return false;
        }
    }


    }
