using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LandingAPI.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnection = configuration.GetConnectionString("DefaultConnection");

        services.AddHealthChecks()
            .AddNpgSql(postgresConnection, name: "postgresql", failureStatus: HealthStatus.Unhealthy);

        services.AddHealthChecksUI(setupSettings: setup =>
        {
            setup.SetEvaluationTimeInSeconds(30);
            setup.MaximumHistoryEntriesPerEndpoint(60);
            setup.AddHealthCheckEndpoint("API", "/health");
        })
            .AddPostgreSqlStorage(configuration.GetConnectionString("DefaultConnection"));


        return services;
    }

    public static WebApplication UseHealthChecksAndUI(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui";
            options.ApiPath = "/health-ui-api";
        });

        return app;
    }
}
