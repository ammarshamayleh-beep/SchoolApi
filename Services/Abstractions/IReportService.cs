using SchoolApi.Auth;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Reports;

namespace SchoolApi.Services.Abstractions;

public interface IReportService
{
    Task<BaseResponse<StudentReportDto>> GetStudentReportAsync(Guid studentId, string? term, CurrentUser user);
    Task<BaseResponse<List<SubjectAverageDto>>> GetSubjectAveragesAsync(string? term);
    Task<BaseResponse<List<ClassroomAverageDto>>> GetClassroomAveragesAsync(string? term);
    Task<BaseResponse<List<TopStudentDto>>> GetTopStudentsAsync(string? term, int count);
}
