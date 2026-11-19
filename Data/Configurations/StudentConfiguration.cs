using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApi.Models;

namespace SchoolApi.Data.Configurations;

public class StudentConfiguration : BaseEntityConfiguration<Student>
{
    public override void Configure(EntityTypeBuilder<Student> builder)
    {
        base.Configure(builder);   // Id + Soft Delete filter
        // Columns
        builder.Property(s => s.FirstName).HasMaxLength(50);
        builder.Property(s => s.LastName).HasMaxLength(50);
        builder.Property(s => s.Email).HasMaxLength(100);

        // Unique email
        builder.HasIndex(s => s.Email).IsUnique().HasFilter("[IsDeleted] = 0"); ;

        // A classroom with students can't be deleted
        builder.HasOne(s => s.Classroom)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.ClassroomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}