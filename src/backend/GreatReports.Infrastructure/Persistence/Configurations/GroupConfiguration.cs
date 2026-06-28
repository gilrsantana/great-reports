using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreatReports.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Timezone)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(g => g.GroupLeaderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ClientCompany>()
            .WithMany()
            .HasForeignKey(g => g.ClientCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(g => g.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany<ScheduledEmail>()
            .WithOne()
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
