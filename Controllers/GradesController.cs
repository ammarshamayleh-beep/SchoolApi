using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Auth;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Grades;
using SchoolApi.Extensions;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Controllers;

[Route("api/[controller]")]
[Authorize]   // everyone must log in. What you SEE depends on your role (see GradeService).
public class GradesController : BaseApiController
{
    private readonly IGradeService _gradeService;

    public GradesController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }

    // GET api/grades?studentId=...&subjectId=...&term=2026-T1&page=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PagedResult<GradeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GradeQuery query)
    {
        return ToActionResult(await _gradeService.GetAllAsync(query, User.ToCurrentUser()));
    }

    // GET api/grades/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<GradeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return ToActionResult(await _gradeService.GetByIdAsync(id, User.ToCurrentUser()));
    }

    // POST api/grades
    [Authorize(Roles = Roles.AdminOrTeacher)]
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<GradeDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] GradeRequestDto dto)
    {
        return ToActionResult(await _gradeService.CreateAsync(dto, User.ToCurrentUser()));
    }

    // PUT api/grades/{id}   (body: { "score": 88.5 })
    [Authorize(Roles = Roles.AdminOrTeacher)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] GradeUpdateDto dto)
    {
        return ToActionResult(await _gradeService.UpdateAsync(id, dto, User.ToCurrentUser()));
    }

    // DELETE api/grades/{id}
    [Authorize(Roles = Roles.AdminOrTeacher)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return ToActionResult(await _gradeService.DeleteAsync(id, User.ToCurrentUser()));
    }
}
