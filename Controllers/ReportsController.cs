using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Auth;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Reports;
using SchoolApi.Extensions;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Controllers;

[Route("api/[controller]")]
[Authorize]   // every report needs a logged-in user
public class ReportsController : BaseApiController
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // GET api/reports/students/{id}?term=2026-T1
    [HttpGet("students/{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<StudentReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> StudentReport([FromRoute] Guid id, [FromQuery] string? term)
    {
        return ToActionResult(await _reportService.GetStudentReportAsync(id, term, User.ToCurrentUser()));
    }

    // GET api/reports/subjects?term=2026-T1
    [Authorize(Roles = Roles.AdminOrTeacher)]
    [HttpGet("subjects")]
    [ProducesResponseType(typeof(BaseResponse<List<SubjectAverageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SubjectAverages([FromQuery] string? term)
    {
        return ToActionResult(await _reportService.GetSubjectAveragesAsync(term));
    }

    // GET api/reports/classrooms?term=2026-T1
    [Authorize(Roles = Roles.AdminOrTeacher)]
    [HttpGet("classrooms")]
    [ProducesResponseType(typeof(BaseResponse<List<ClassroomAverageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ClassroomAverages([FromQuery] string? term)
    {
        return ToActionResult(await _reportService.GetClassroomAveragesAsync(term));
    }

    // GET api/reports/top-students?term=2026-T1&count=5
    [Authorize(Roles = Roles.AdminOrTeacher)]
    [HttpGet("top-students")]
    [ProducesResponseType(typeof(BaseResponse<List<TopStudentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TopStudents([FromQuery] TopStudentsQuery query)
    {
        return ToActionResult(await _reportService.GetTopStudentsAsync(query.Term, query.Count));
    }
}
