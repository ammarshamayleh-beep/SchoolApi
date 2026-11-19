using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApi.Models;

namespace SchoolApi.Data.Configurations;

// The rules that every entity shares
public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        // Primary key (instead of [Key] in BaseEntity)
        builder.HasKey(e => e.Id);

        // Soft Delete: hide deleted rows from every query
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}