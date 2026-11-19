using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Auth;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Subjects;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Controllers;

[Route("api/[controller]")]
[Authorize]   // reading needs a login; changing needs Admin
public class SubjectsController : BaseApiController
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    // GET api/subjects?page=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PagedResult<SubjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
    {
        return ToActionResult(await _subjectService.GetAllAsync(query));
    }

    // GET api/subjects/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<SubjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return ToActionResult(await _subjectService.GetByIdAsync(id));
    }

    // POST api/subjects
    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<SubjectDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] SubjectRequestDto dto)
    {
        return ToActionResult(await _subjectService.CreateAsync(dto));
    }

    // PUT api/subjects/{id}
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] SubjectRequestDto dto)
    {
        return ToActionResult(await _subjectService.UpdateAsync(id, dto));
    }

    // DELETE api/subjects/{id}
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return ToActionResult(await _subjectService.DeleteAsync(id));
    }
}
