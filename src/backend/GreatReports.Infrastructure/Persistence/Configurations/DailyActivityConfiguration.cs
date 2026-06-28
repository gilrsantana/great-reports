using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreatReports.Infrastructure.Persistence.Configurations;

public class DailyActivityConfiguration : IEntityTypeConfiguration<DailyActivity>
{
    public void Configure(EntityTypeBuilder<DailyActivity> builder)
    {
        builder.ToTable("DailyActivities");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Theme)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.ReferenceDate)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.IsBlocked)
            .IsRequired();

        builder.Property(e => e.IsPublished)
            .IsRequired();

        builder.Property(e => e.SummarizedContent)
            .HasMaxLength(4000);

        builder.Property(e => e.ProcessedAt);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(da => da.PartnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
