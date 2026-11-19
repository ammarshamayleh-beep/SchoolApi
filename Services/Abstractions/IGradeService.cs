using SchoolApi.Auth;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Grades;

namespace SchoolApi.Services.Abstractions;

public interface IGradeService
{
    Task<BaseResponse<PagedResult<GradeDto>>> GetAllAsync(GradeQuery query, CurrentUser user);
    Task<BaseResponse<GradeDto>> GetByIdAsync(Guid id, CurrentUser user);
    Task<BaseResponse<GradeDto>> CreateAsync(GradeRequestDto dto, CurrentUser user);
    Task<BaseResponse<bool>> UpdateAsync(Guid id, GradeUpdateDto dto, CurrentUser user);
    Task<BaseResponse<bool>> DeleteAsync(Guid id, CurrentUser user);
}
