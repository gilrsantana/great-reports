using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreatReports.Infrastructure.Persistence.Configurations;

public class ScheduledEmailReceiverConfiguration : IEntityTypeConfiguration<ScheduledEmailReceiver>
{
    public void Configure(EntityTypeBuilder<ScheduledEmailReceiver> builder)
    {
        builder.ToTable("ScheduledEmailReceivers");

        builder.HasKey(e => e.Id);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ClientContact>()
            .WithMany()
            .HasForeignKey(r => r.ClientContactId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
