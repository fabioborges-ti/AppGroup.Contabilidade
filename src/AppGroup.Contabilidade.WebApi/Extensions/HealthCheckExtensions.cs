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
                healthQuery: "SELECT 1",
                failureStatus: HealthStatus.Degraded,
                tags: ["db", "sql", "sqlserver"]
            );

        services
            .AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(15); // tempo entre verificações
                options.MaximumHistoryEntriesPerEndpoint(60);
                options.SetHeaderText("Contabilidade API - Health Checks");
                options.AddHealthCheckEndpoint("API", "/health");
            })
            .AddInMemoryStorage();

        return services;
    }
}