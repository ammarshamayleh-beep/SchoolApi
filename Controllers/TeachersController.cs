using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Auth;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Teachers;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Controllers;

[Route("api/[controller]")]
[Authorize]   // reading needs a login; changing needs Admin
public class TeachersController : BaseApiController
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    // GET api/teachers?page=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PagedResult<TeacherDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
    {
        return ToActionResult(await _teacherService.GetAllAsync(query));
    }

    // GET api/teachers/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<TeacherDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return ToActionResult(await _teacherService.GetByIdAsync(id));
    }

    // POST api/teachers
    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<TeacherDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] TeacherRequestDto dto)
    {
        return ToActionResult(await _teacherService.CreateAsync(dto));
    }

    // PUT api/teachers/{id}
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] TeacherRequestDto dto)
    {
        return ToActionResult(await _teacherService.UpdateAsync(id, dto));
    }

    // DELETE api/teachers/{id}
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return ToActionResult(await _teacherService.DeleteAsync(id));
    }
}
