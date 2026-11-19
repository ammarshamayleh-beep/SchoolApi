using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Auth;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Students;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = Roles.AdminOrTeacher)]   // students can't list other students
public class StudentsController : BaseApiController
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    // GET api/students?search=ali&classroomId=...&sortBy=dob&desc=true&page=1&pageSize=5
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PagedResult<StudentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] StudentQuery query)
    {
        return ToActionResult(await _studentService.GetAllAsync(query));
    }

    // GET api/students/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<StudentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return ToActionResult(await _studentService.GetByIdAsync(id));
    }

    // POST api/students
    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<StudentDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] StudentRequestDto dto)
    {
        return ToActionResult(await _studentService.CreateAsync(dto));
    }

    // PUT api/students/{id}
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] StudentRequestDto dto)
    {
        return ToActionResult(await _studentService.UpdateAsync(id, dto));
    }

    // DELETE api/students/{id}
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return ToActionResult(await _studentService.DeleteAsync(id));
    }
}
