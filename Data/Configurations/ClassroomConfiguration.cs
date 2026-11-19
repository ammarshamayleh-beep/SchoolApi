using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApi.Models;

namespace SchoolApi.Data.Configurations;

public class ClassroomConfiguration : BaseEntityConfiguration<Classroom>
{
    public override void Configure(EntityTypeBuilder<Classroom> builder)
    {
        base.Configure(builder);   // Id + Soft Delete filter
        builder.Property(c => c.Name).HasMaxLength(50);
    }
}