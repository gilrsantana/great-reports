using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreatReports.Infrastructure.Persistence.Configurations;

public class ScheduledEmailConfiguration : IEntityTypeConfiguration<ScheduledEmail>
{
    public void Configure(EntityTypeBuilder<ScheduledEmail> builder)
    {
        builder.ToTable("ScheduledEmails");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.CronExpression)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Frequency)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.SpecificDayOfMonth);

        builder.HasMany<ScheduledEmailReceiver>()
            .WithOne()
            .HasForeignKey(r => r.ScheduledEmailId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
