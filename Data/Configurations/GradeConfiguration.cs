using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApi.Models;

namespace SchoolApi.Data.Configurations;

public class GradeConfiguration : BaseEntityConfiguration<Grade>
{
    public override void Configure(EntityTypeBuilder<Grade> builder)
    {
        base.Configure(builder);   // Id + Soft Delete filter
        // Columns
        builder.Property(g => g.Term).HasMaxLength(20);
        builder.Property(g => g.Score).HasPrecision(5, 2);

        // The score must be between 0 and 100
        builder.ToTable(t => t.HasCheckConstraint("CK_Grades_Score", "[Score] BETWEEN 0 AND 100"));

        // One student can have only one grade per subject per term
        builder.HasIndex(g => new { g.StudentId, g.SubjectId, g.Term }).IsUnique().HasFilter("[IsDeleted] = 0"); ;

        // If a student is deleted, his grades are deleted too
        builder.HasOne(g => g.Student)
            .WithMany(s => s.Grades)
            .HasForeignKey(g => g.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // A subject with grades can't be deleted
        builder.HasOne(g => g.Subject)
            .WithMany(s => s.Grades)
            .HasForeignKey(g => g.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}