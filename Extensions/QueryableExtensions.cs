using Microsoft.EntityFrameworkCore;
using SchoolApi.DTOs.Common;

namespace SchoolApi.Extensions;

public static class QueryableExtensions
{
    // Counts all the rows, then takes one page of them
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, PaginationQuery pagination)
    {
        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }
}
