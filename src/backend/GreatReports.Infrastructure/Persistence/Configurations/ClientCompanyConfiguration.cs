using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreatReports.Infrastructure.Persistence.Configurations;

public class ClientCompanyConfiguration : IEntityTypeConfiguration<ClientCompany>
{
    public void Configure(EntityTypeBuilder<ClientCompany> builder)
    {
        builder.ToTable("ClientCompanies");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany<Project>()
            .WithOne()
            .HasForeignKey(p => p.ClientCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany<ClientContact>()
            .WithOne()
            .HasForeignKey(c => c.ClientCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
