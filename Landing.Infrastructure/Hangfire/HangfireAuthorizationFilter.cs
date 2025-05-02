using Hangfire.Dashboard;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Landing.Core.Models;

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
        };
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        var token = httpContext.Request.Cookies["hangfire_admin_token"];
        if (string.IsNullOrEmpty(token))
            return false;

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var principal = handler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
            return principal.IsInRole("Admin");
        }
        catch
        {
            return false;
        }
    }
}
