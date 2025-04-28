using Hangfire.Dashboard;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Landing.Core.Models
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly TokenValidationParameters _tokenValidationParameters;

        public HangfireAuthorizationFilter(IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
            };
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var path = httpContext.Request.Path.Value?.ToLowerInvariant();

            // Разрешаем грузить статику без авторизации
            if (path != null && (
                path.StartsWith("/hangfire/css") ||
                path.StartsWith("/hangfire/js") ||
                path.StartsWith("/hangfire/fonts")
            ))
            {
                return true;
            }

            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                    return principal.Identity?.IsAuthenticated == true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }
}
