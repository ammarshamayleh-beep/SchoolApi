using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApi.Models;

namespace SchoolApi.Data.Configurations;

public class TeacherConfiguration : BaseEntityConfiguration<Teacher>
{
    public override void Configure(EntityTypeBuilder<Teacher> builder)
    {
        base.Configure(builder);   // Id + Soft Delete filter
        // Columns
        builder.Property(t => t.FirstName).HasMaxLength(50);
        builder.Property(t => t.LastName).HasMaxLength(50);
        builder.Property(t => t.Email).HasMaxLength(100);

        // Unique email
        builder.HasIndex(t => t.Email).IsUnique().HasFilter("[IsDeleted] = 0"); ;
    }
}