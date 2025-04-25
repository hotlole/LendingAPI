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
    public class HangfireAuthorizationFilter(IConfiguration config, IServiceProvider serviceProvider) : IDashboardAuthorizationFilter
    {
        private readonly IConfiguration _config = config;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return false;

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_config["Jwt:Key"]!))
                }, out _);

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return false;

                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var user = db.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefault(u => u.Id.ToString() == userIdClaim.Value);

                return user?.UserRoles.Any(r => r.Role.Name == "Admin") == true;
            }
            catch
            {
                return false;
            }
        }
    }


}
