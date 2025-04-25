using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Infrastructure.Database.Data.Context;
using AppGroup.Contabilidade.Infrastructure.Database.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppGroup.Contabilidade.Infrastructure.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        #region Repositories

        services.AddScoped<IContaContabilRepository, ContaContabilRepository>();

        #endregion Repositories

        return services;
    }
}