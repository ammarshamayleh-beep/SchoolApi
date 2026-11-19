namespace SchoolApi.Services.Abstractions;

// Gives the id of the logged-in user to any class that needs it (like SchoolContext)
public interface ICurrentUserService
{
    Guid UserId { get; }   // Guid.Empty = no user (the seeder, or a request without a token)
}
