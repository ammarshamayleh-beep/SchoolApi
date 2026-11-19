namespace SchoolApi.DTOs.Common;

// The page options that every list can take: api/xxx?page=2&pageSize=10
public class PaginationQuery
{

    public int Page { get; set; } = 1;


    public int PageSize { get; set; } = 10;
}
