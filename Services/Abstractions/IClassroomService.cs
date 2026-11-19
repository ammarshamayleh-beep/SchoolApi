using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Classrooms;

namespace SchoolApi.Services.Abstractions;

public interface IClassroomService
{
    Task<BaseResponse<PagedResult<ClassroomDto>>> GetAllAsync(PaginationQuery query);
    Task<BaseResponse<ClassroomDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<ClassroomDto>> CreateAsync(ClassroomRequestDto dto);
    Task<BaseResponse<bool>> UpdateAsync(Guid id, ClassroomRequestDto dto);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
}
