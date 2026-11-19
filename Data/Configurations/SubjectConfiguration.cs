using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApi.Models;

namespace SchoolApi.Data.Configurations;

public class SubjectConfiguration : BaseEntityConfiguration<Subject>
{
    public override void Configure(EntityTypeBuilder<Subject> builder)
    {
        base.Configure(builder);   // Id + Soft Delete filter
        // Columns
        builder.Property(s => s.Name).HasMaxLength(100);
        builder.Property(s => s.Code).HasMaxLength(20);

        // Unique code
        builder.HasIndex(s => s.Code).IsUnique().HasFilter("[IsDeleted] = 0"); ;

        // If a teacher leaves, his subjects stay without a teacher
        builder.HasOne(s => s.Teacher)
            .WithMany(t => t.Subjects)
            .HasForeignKey(s => s.TeacherId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}