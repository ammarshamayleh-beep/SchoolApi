using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Students;

namespace SchoolApi.Services.Abstractions;

public interface IStudentService
{
    Task<BaseResponse<PagedResult<StudentDto>>> GetAllAsync(StudentQuery query);
    Task<BaseResponse<StudentDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<StudentDto>> CreateAsync(StudentRequestDto dto);
    Task<BaseResponse<bool>> UpdateAsync(Guid id, StudentRequestDto dto);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
}
