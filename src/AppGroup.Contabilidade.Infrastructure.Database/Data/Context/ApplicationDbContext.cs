using AppGroup.Contabilidade.Domain.Entities;
using AppGroup.Contabilidade.Domain.Enums;
using AppGroup.Contabilidade.Infrastructure.Database.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AppGroup.Contabilidade.Infrastructure.Database.Data.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.ApplyConfiguration(new ContaContabilConfiguration());

        #region SEEDS

        builder.Entity<ContaContabil>().HasData(new ContaContabil
        {
            Id = Guid.Parse("55ad855d-c9e8-4566-8ef0-fe189acc0533"),
            Codigo = "1",
            Nome = "Receitas",
            Tipo = TipoConta.Receitas,
            AceitaLancamentos = false,
        });

        builder.Entity<ContaContabil>().HasData(new ContaContabil
        {
            Id = Guid.Parse("f1ceaafe-3d8f-4dbc-aac0-e1e27f8ec436"),
            Codigo = "2",
            Nome = "Despesas",
            Tipo = TipoConta.Despesas,
            AceitaLancamentos = false,
        });

        #endregion SEEDS

        base.OnModelCreating(builder);
    }
}
