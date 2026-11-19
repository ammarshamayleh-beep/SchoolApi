using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Subjects;

namespace SchoolApi.Services.Abstractions;

public interface ISubjectService
{
    Task<BaseResponse<PagedResult<SubjectDto>>> GetAllAsync(PaginationQuery query);
    Task<BaseResponse<SubjectDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<SubjectDto>> CreateAsync(SubjectRequestDto dto);
    Task<BaseResponse<bool>> UpdateAsync(Guid id, SubjectRequestDto dto);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
}
