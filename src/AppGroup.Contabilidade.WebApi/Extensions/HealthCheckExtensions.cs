using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AppGroup.Contabilidade.WebApi.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection ConfigureHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddSqlServer(
                connectionString: configuration.GetConnectionString("DefaultConnection")!,
                name: "SqlServer",
                failureStatus: HealthStatus.Degraded,
                tags: ["db", "sql", "sqlserver"]
            );

        return services;
    }
}
