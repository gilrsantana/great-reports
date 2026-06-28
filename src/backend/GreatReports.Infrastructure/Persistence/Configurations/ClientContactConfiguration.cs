using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreatReports.Infrastructure.Persistence.Configurations;

public class ClientContactConfiguration : IEntityTypeConfiguration<ClientContact>
{
    public void Configure(EntityTypeBuilder<ClientContact> builder)
    {
        builder.ToTable("ClientContacts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.Property(e => e.VerificationToken)
            .HasMaxLength(100);

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }
}
