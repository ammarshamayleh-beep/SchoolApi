using Microsoft.EntityFrameworkCore;
using SchoolApi.Models;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Data;

public class SchoolContext : DbContext
{
    private readonly ICurrentUserService _currentUser;

    // The options (provider + connection string) come from Program.cs
    // ICurrentUserService tells us who is logged in, for the audit columns
    public SchoolContext(DbContextOptions<SchoolContext> options, ICurrentUserService currentUser) : base(options)
    {
        _currentUser = currentUser;
    }

    // Our tables
    public DbSet<Classroom> Classrooms { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Load all configuration classes from Data/Configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolContext).Assembly);


    }


    // Every SaveChangesAsync in the project passes through here first
    // Every SaveChangesAsync in the project passes through here first
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditAndSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditAndSoftDelete()
    {
        DateTime now = DateTime.UtcNow;
        Guid userId = _currentUser.UserId;   // Guid.Empty = the system (for example the seeder)

        // ChangeTracker: the list of entities that EF is about to save
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = userId;
                entry.Entity.CreatedDate = now;
                entry.Entity.LastUpdatedBy = userId;
                entry.Entity.LastUpdatedDate = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastUpdatedBy = userId;
                entry.Entity.LastUpdatedDate = now;
            }
            else if (entry.State == EntityState.Deleted)
            {
                // Soft Delete: turn the DELETE into an UPDATE
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.LastUpdatedBy = userId;
                entry.Entity.LastUpdatedDate = now;
            }
        }
    }
}