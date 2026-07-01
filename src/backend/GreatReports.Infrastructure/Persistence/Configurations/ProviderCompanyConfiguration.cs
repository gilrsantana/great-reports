using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreatReports.Infrastructure.Persistence.Configurations;

public class ProviderCompanyConfiguration : IEntityTypeConfiguration<ProviderCompany>
{
    public void Configure(EntityTypeBuilder<ProviderCompany> builder)
    {
        builder.ToTable("ProviderCompanies");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.TaxId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.TaxId)
            .IsUnique();

        builder.Property(e => e.ManagerId)
        .IsRequired();

        builder.HasMany<ClientCompany>()
            .WithOne()
            .HasForeignKey(c => c.ProviderCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany<User>()
            .WithOne()
            .HasForeignKey(u => u.ProviderCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
