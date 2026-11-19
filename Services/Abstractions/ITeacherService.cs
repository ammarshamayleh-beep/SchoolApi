using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Teachers;

namespace SchoolApi.Services.Abstractions;

public interface ITeacherService
{
    Task<BaseResponse<PagedResult<TeacherDto>>> GetAllAsync(PaginationQuery query);
    Task<BaseResponse<TeacherDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<TeacherDto>> CreateAsync(TeacherRequestDto dto);
    Task<BaseResponse<bool>> UpdateAsync(Guid id, TeacherRequestDto dto);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
}
