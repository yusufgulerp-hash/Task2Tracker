using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Infrastructure.Persistence.Configurations;

public sealed class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("ProjectMembers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProjectId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne<Project>()
            .WithMany(p => p.Members)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

     
        builder.HasIndex(x => new { x.ProjectId, x.UserId }).IsUnique();

        builder.HasIndex(x => x.UserId);
    }
}
