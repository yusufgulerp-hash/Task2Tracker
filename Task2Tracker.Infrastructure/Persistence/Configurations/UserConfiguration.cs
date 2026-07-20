using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_User_FirstName_NoNumbers", "\"FirstName\" !~ '[0-9]'");
            t.HasCheckConstraint("CK_User_LastName_NoNumbers", "\"LastName\" !~ '[0-9]'");
        });

        builder.Metadata
            .FindNavigation(nameof(User.RefreshTokens))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}