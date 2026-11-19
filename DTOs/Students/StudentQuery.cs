using SchoolApi.DTOs.Common;

namespace SchoolApi.DTOs.Students;

// The options the client can send in the query string for GET api/students
public class StudentQuery : PaginationQuery
{
    // Filtering
    public string? Search { get; set; }        // first name, last name or email
    public Guid? ClassroomId { get; set; }      // only students of this classroom

    // Sorting: "name" (default) or "dob"
    public string? SortBy { get; set; }
    public bool Desc { get; set; }
}
