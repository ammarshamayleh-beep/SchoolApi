using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SchoolApi.Extensions;

public static class DbUpdateExceptionExtensions
{
    // SQL Server error numbers for a duplicate value
    private const int UniqueIndexViolation = 2601;       // a unique index (our HasIndex(...).IsUnique())
    private const int UniqueConstraintViolation = 2627;  // a unique constraint or primary key

    // True when SaveChanges failed because a unique index rejected a duplicate value.
    // So we don't need an extra "AnyAsync" query before saving: the database checks it for us.
    public static bool IsUniqueViolation(this DbUpdateException exception)
    {
        return exception.InnerException is SqlException { Number: UniqueIndexViolation or UniqueConstraintViolation };
    }
}
