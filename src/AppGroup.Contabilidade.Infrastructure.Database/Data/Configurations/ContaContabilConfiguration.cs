using AppGroup.Contabilidade.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppGroup.Contabilidade.Infrastructure.Database.Data.Configurations;

public class ContaContabilConfiguration : IEntityTypeConfiguration<ContaContabil>
{
    public void Configure(EntityTypeBuilder<ContaContabil> builder)
    {
        builder.ToTable("ContasContabeis");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .IsRequired()
               .HasDefaultValueSql("NEWID()");

        builder.Property(c => c.Codigo)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(c => c.Nome)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(c => c.Tipo)
               .HasMaxLength(50);

        builder.Property(c => c.AceitaLancamentos);

        builder.Property(c => c.IdPai);

        builder.HasOne(c => c.ContaPai)
               .WithMany(c => c.SubContas)
               .HasForeignKey(c => c.IdPai)
               .OnDelete(DeleteBehavior.NoAction);
    }
}