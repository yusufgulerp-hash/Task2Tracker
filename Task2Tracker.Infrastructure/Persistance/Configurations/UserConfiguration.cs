using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // BaseEntity'den gelen birincil anahtar
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.Email).IsUnique();

        // 🌟 PostgreSQL seviyesinde sayıları engelleyen Check Constraint koruması
        builder.ToTable(t => t.HasCheckConstraint("CK_User_FirstName_NoNumbers", "\"FirstName\" !~ '[0-9]'"));
        builder.ToTable(t => t.HasCheckConstraint("CK_User_LastName_NoNumbers", "\"LastName\" !~ '[0-9]'"));
    }
}