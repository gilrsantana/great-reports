using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreatReports.Infrastructure.Persistence.Configurations;

public class EmailAuditLogConfiguration : IEntityTypeConfiguration<EmailAuditLog>
{
    public void Configure(EntityTypeBuilder<EmailAuditLog> builder)
    {
        builder.ToTable("EmailAuditLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Recipient)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.Subject)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Body)
            .IsRequired()
            .HasMaxLength(8000);

        builder.Property(e => e.SentAt)
            .IsRequired();

        builder.Property(e => e.Success)
            .IsRequired();

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);
    }
}
