using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Auth;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Classrooms;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Controllers;

[Route("api/[controller]")]
[Authorize]   // reading needs a login; changing needs Admin
public class ClassroomsController : BaseApiController
{
    private readonly IClassroomService _classroomService;

    public ClassroomsController(IClassroomService classroomService)
    {
        _classroomService = classroomService;
    }

    // GET api/classrooms?page=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PagedResult<ClassroomDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
    {
        return ToActionResult(await _classroomService.GetAllAsync(query));
    }

    // GET api/classrooms/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<ClassroomDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return ToActionResult(await _classroomService.GetByIdAsync(id));
    }

    // POST api/classrooms
    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<ClassroomDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] ClassroomRequestDto dto)
    {
        return ToActionResult(await _classroomService.CreateAsync(dto));
    }

    // PUT api/classrooms/{id}
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ClassroomRequestDto dto)
    {
        return ToActionResult(await _classroomService.UpdateAsync(id, dto));
    }

    // DELETE api/classrooms/{id}
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return ToActionResult(await _classroomService.DeleteAsync(id));
    }
}
