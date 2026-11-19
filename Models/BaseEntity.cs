namespace SchoolApi.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid LastUpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        // Soft Delete: true = deleted (the row stays in the database)
        public bool IsDeleted { get; set; }
    }
}