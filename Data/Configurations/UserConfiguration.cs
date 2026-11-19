using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApi.Models;

namespace SchoolApi.Data.Configurations;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);   // Id + Soft Delete filter
        // Columns
        builder.Property(u => u.Email).HasMaxLength(100);
        builder.Property(u => u.PasswordHash).HasMaxLength(200);
        builder.Property(u => u.Role).HasMaxLength(20);

        // Unique email
        builder.HasIndex(u => u.Email).IsUnique().HasFilter("[IsDeleted] = 0"); ;

        // A teacher has one account at most. If the teacher is deleted, his account goes too.
        builder.HasOne(u => u.Teacher)
            .WithOne()
            .HasForeignKey<User>(u => u.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        // Same for students
        builder.HasOne(u => u.Student)
            .WithOne()
            .HasForeignKey<User>(u => u.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}