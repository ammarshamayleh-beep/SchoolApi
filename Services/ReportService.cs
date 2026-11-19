using Microsoft.EntityFrameworkCore;
using SchoolApi.Auth;
using SchoolApi.Data;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Reports;
using SchoolApi.Models;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Services;

public class ReportService : IReportService
{
    private readonly SchoolContext _context;

    public ReportService(SchoolContext context)
    {
        _context = context;
    }

    // Database trips: 2 (the student + his grades)
    public async Task<BaseResponse<StudentReportDto>> GetStudentReportAsync(Guid studentId, string? term, CurrentUser user)
    {
        // Rule: a student can only see his own report
        if (user.IsStudent && user.StudentId != studentId)
        {
            return BaseResponse<StudentReportDto>.Failure(StatusCodes.Status403Forbidden, "Students can only see their own report.");
        }

        // 1. The student's info
        var report = await _context.Students
            .Where(s => s.Id == studentId)
            .Select(s => new StudentReportDto
            {
                StudentId = s.Id,
                StudentName = s.FirstName + " " + s.LastName,
                ClassroomName = s.Classroom!.Name
            })
            .FirstOrDefaultAsync();

        if (report == null)
        {
            return BaseResponse<StudentReportDto>.Failure(StatusCodes.Status404NotFound, $"Student {studentId} was not found.");
        }

        // 2. His grades (one term, or all terms)
        IQueryable<Grade> grades = FilterByTerm(_context.Grades, term)
            .Where(g => g.StudentId == studentId);

        report.Term = term;
        report.Grades = await grades
            .OrderBy(g => g.Term)
            .ThenBy(g => g.Subject!.Code)
            .Select(g => new SubjectScoreDto
            {
                SubjectCode = g.Subject!.Code,
                SubjectName = g.Subject.Name,
                Term = g.Term,
                Score = g.Score
            })
            .ToListAsync();

        // 3. His average, from the grades we already loaded (no third query)
        report.Average = report.Grades.Count == 0
            ? null
            : Math.Round(report.Grades.Average(g => g.Score), 2);

        return BaseResponse<StudentReportDto>.Success(report);
    }

    public async Task<BaseResponse<List<SubjectAverageDto>>> GetSubjectAveragesAsync(string? term)
    {
        // GroupBy: put the grades in groups, one group per subject,
        // then calculate numbers for each group (GROUP BY in SQL)
        var averages = await FilterByTerm(_context.Grades, term)
            .GroupBy(g => new { g.SubjectId, g.Subject!.Code, g.Subject.Name })
            .Select(group => new SubjectAverageDto
            {
                SubjectId = group.Key.SubjectId,
                SubjectCode = group.Key.Code,
                SubjectName = group.Key.Name,
                GradeCount = group.Count(),
                Average = Math.Round(group.Average(g => g.Score), 2),
                Lowest = group.Min(g => g.Score),
                Highest = group.Max(g => g.Score)
            })
            .OrderByDescending(s => s.Average)
            .ToListAsync();

        return BaseResponse<List<SubjectAverageDto>>.Success(averages);
    }

    public async Task<BaseResponse<List<ClassroomAverageDto>>> GetClassroomAveragesAsync(string? term)
    {
        // One group per classroom (through the student of each grade)
        var averages = await FilterByTerm(_context.Grades, term)
            .GroupBy(g => new { g.Student!.ClassroomId, g.Student.Classroom!.Name })
            .Select(group => new ClassroomAverageDto
            {
                ClassroomId = group.Key.ClassroomId,
                ClassroomName = group.Key.Name,
                StudentCount = group.Select(g => g.StudentId).Distinct().Count(),
                Average = Math.Round(group.Average(g => g.Score), 2)
            })
            .OrderByDescending(c => c.Average)
            .ToListAsync();

        return BaseResponse<List<ClassroomAverageDto>>.Success(averages);
    }

    public async Task<BaseResponse<List<TopStudentDto>>> GetTopStudentsAsync(string? term, int count)
    {
        // One group per student, sorted by average, then take the first ones
        var students = await FilterByTerm(_context.Grades, term)
            .GroupBy(g => new
            {
                g.StudentId,
                g.Student!.FirstName,
                g.Student.LastName,
                ClassroomName = g.Student.Classroom!.Name
            })
            .Select(group => new TopStudentDto
            {
                StudentId = group.Key.StudentId,
                StudentName = group.Key.FirstName + " " + group.Key.LastName,
                ClassroomName = group.Key.ClassroomName,
                Average = Math.Round(group.Average(g => g.Score), 2)
            })
            .OrderByDescending(s => s.Average)
            .Take(count)
            .ToListAsync();

        return BaseResponse<List<TopStudentDto>>.Success(students);
    }

    // All the reports can be for one term, or for all terms
    private static IQueryable<Grade> FilterByTerm(IQueryable<Grade> grades, string? term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return grades;
        }

        return grades.Where(g => g.Term == term);
    }
}
