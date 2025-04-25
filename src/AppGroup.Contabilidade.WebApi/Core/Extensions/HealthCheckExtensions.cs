using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AppGroup.Contabilidade.WebApi.Core.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection ConfigureHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddNpgSql
            (
                //npgsqlConnectionString: configuration.GetConnectionString("DefaultConnection"),
                name: "Postgres",
                failureStatus: HealthStatus.Degraded,
                tags: new string[] { "db", "sql", "postgres" }
            );

        return services;
    }
}