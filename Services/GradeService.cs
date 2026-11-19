using Microsoft.EntityFrameworkCore;
using SchoolApi.Auth;
using SchoolApi.Data;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Grades;
using SchoolApi.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SchoolApi.Models;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Services;

public class GradeService : IGradeService
{
    private readonly SchoolContext _context;
    private readonly ILogger<GradeService> _logger;
    private readonly IMapper _mapper;

    public GradeService(SchoolContext context, ILogger<GradeService> logger, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BaseResponse<PagedResult<GradeDto>>> GetAllAsync(GradeQuery q, CurrentUser user)
    {
        // Start from the grades this user is allowed to see
        IQueryable<Grade> query = VisibleTo(_context.Grades, user);

        // Then the filters the client sent
        if (q.StudentId != null)
        {
            Guid studentId = q.StudentId.Value;
            query = query.Where(g => g.StudentId == studentId);
        }

        if (q.SubjectId != null)
        {
            Guid subjectId = q.SubjectId.Value;
            query = query.Where(g => g.SubjectId == subjectId);
        }

        string? term = q.Term;
        if (!string.IsNullOrWhiteSpace(term))
        {
            query = query.Where(g => g.Term == term);
        }

        var page = await query
            .OrderBy(g => g.Student!.FirstName)
            .ThenBy(g => g.Student!.LastName)
            .ThenBy(g => g.Subject!.Code)
            .ProjectTo<GradeDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(q);

        return BaseResponse<PagedResult<GradeDto>>.Success(page);
    }

    public async Task<BaseResponse<GradeDto>> GetByIdAsync(Guid id, CurrentUser user)
    {
        var grade = await FindVisibleAsync(id, user);

        // A grade you can't see looks like it doesn't exist (404)
        if (grade == null)
        {
            return BaseResponse<GradeDto>.Failure(StatusCodes.Status404NotFound, $"Grade {id} was not found.");
        }

        return BaseResponse<GradeDto>.Success(grade);
    }

    // Database trips: 2 (one query for rules 1-3 + the INSERT)
    public async Task<BaseResponse<GradeDto>> CreateAsync(GradeRequestDto dto, CurrentUser user)
    {
        // One query for rules 1, 2 and 3: the subject (its code and its teacher) and the student's name.
        // The response needs the subject code and the student name too, so we keep them.
        var found = await _context.Subjects
            .Where(s => s.Id == dto.SubjectId)
            .Select(s => new
            {
                s.Code,
                s.TeacherId,
                StudentName = _context.Students
                    .Where(st => st.Id == dto.StudentId)
                    .Select(st => st.FirstName + " " + st.LastName)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        // Rule 1: the subject must exist
        if (found == null)
        {
            return BaseResponse<GradeDto>.Failure(StatusCodes.Status400BadRequest, $"Subject {dto.SubjectId} was not found.");
        }

        // Rule 2: the student must exist
        if (found.StudentName == null)
        {
            return BaseResponse<GradeDto>.Failure(StatusCodes.Status400BadRequest, $"Student {dto.StudentId} was not found.");
        }

        // Rule 3: a teacher can only add grades to his own subjects (checked in memory, no extra query)
        if (!CanEditSubject(found.TeacherId, user))
        {
            _logger.LogWarning("User {UserId} tried to add a grade to subject {SubjectId}", user.UserId, dto.SubjectId);
            return BaseResponse<GradeDto>.Failure(StatusCodes.Status403Forbidden, "You can only add grades to your own subjects.");
        }

        var grade = _mapper.Map<Grade>(dto);

        _context.Grades.Add(grade);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            // Rule 4: one grade per student, per subject, per term.
            // The unique index (StudentId, SubjectId, Term) checked it, so no AnyAsync before saving.
            return BaseResponse<GradeDto>.Failure(StatusCodes.Status409Conflict,
                $"Student {dto.StudentId} already has a grade in subject {dto.SubjectId} for term {dto.Term}.");
        }

        _logger.LogInformation("User {UserId} added grade {GradeId} with score {Score}", user.UserId, grade.Id, grade.Score);

        // Build the DTO in memory (no second query), with the names we already have
        var created = _mapper.Map<GradeDto>(grade);
        created.StudentName = found.StudentName;
        created.SubjectCode = found.Code;

        return BaseResponse<GradeDto>.Success(created, StatusCodes.Status201Created);
    }

    // Database trips: 2 (one query for the grade and its subject's teacher + UPDATE)
    public async Task<BaseResponse<bool>> UpdateAsync(Guid id, GradeUpdateDto dto, CurrentUser user)
    {
        // One query: the grade itself, and the teacher of its subject (for the permission check)
        var found = await _context.Grades
            .Where(g => g.Id == id)
            .Select(g => new { Grade = g, SubjectTeacherId = g.Subject!.TeacherId })
            .FirstOrDefaultAsync();

        if (found == null)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status404NotFound, $"Grade {id} was not found.");
        }

        // A teacher can only change grades of his own subjects
        if (!CanEditSubject(found.SubjectTeacherId, user))
        {
            _logger.LogWarning("User {UserId} tried to change grade {GradeId}", user.UserId, id);
            return BaseResponse<bool>.Failure(StatusCodes.Status403Forbidden, "You can only change grades of the subjects you teach.");
        }

        decimal oldScore = found.Grade.Score;

        _mapper.Map(dto, found.Grade);
        await _context.SaveChangesAsync();

        // Changing a grade is important, so we keep a record of it in the log
        _logger.LogInformation("User {UserId} changed grade {GradeId} from {OldScore} to {NewScore}",
            user.UserId, id, oldScore, found.Grade.Score);

        return BaseResponse<bool>.Success(true);
    }

    // Database trips: 2 (one query for the grade and its subject's teacher + UPDATE)
    public async Task<BaseResponse<bool>> DeleteAsync(Guid id, CurrentUser user)
    {
        var found = await _context.Grades
            .Where(g => g.Id == id)
            .Select(g => new { Grade = g, SubjectTeacherId = g.Subject!.TeacherId })
            .FirstOrDefaultAsync();

        if (found == null)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status404NotFound, $"Grade {id} was not found.");
        }

        if (!CanEditSubject(found.SubjectTeacherId, user))
        {
            _logger.LogWarning("User {UserId} tried to delete grade {GradeId}", user.UserId, id);
            return BaseResponse<bool>.Failure(StatusCodes.Status403Forbidden, "You can only change grades of the subjects you teach.");
        }

        _context.Grades.Remove(found.Grade);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} deleted grade {GradeId}", user.UserId, id);

        return BaseResponse<bool>.Success(true);
    }

    // One grade as a DTO, only if this user is allowed to see it (null = not found, or not yours)
    private async Task<GradeDto?> FindVisibleAsync(Guid id, CurrentUser user)
    {
        return await VisibleTo(_context.Grades, user)
            .Where(g => g.Id == id)
            .ProjectTo<GradeDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    // Admin sees all grades, a teacher sees his subjects, a student sees his own grades
    private static IQueryable<Grade> VisibleTo(IQueryable<Grade> grades, CurrentUser user)
    {
        if (user.IsAdmin)
        {
            return grades;
        }

        if (user.IsTeacher && user.TeacherId != null)
        {
            var teacherId = user.TeacherId.Value;
            return grades.Where(g => g.Subject!.TeacherId == teacherId);
        }

        if (user.IsStudent && user.StudentId != null)
        {
            var studentId = user.StudentId.Value;
            return grades.Where(g => g.StudentId == studentId);
        }

        return grades.Where(g => false);   // anyone else sees nothing
    }

    // Admin can edit any subject. A teacher only the subjects he teaches.
    // No database call: the caller already read the subject's TeacherId in its own query.
    private static bool CanEditSubject(Guid? subjectTeacherId, CurrentUser user)
    {
        if (user.IsAdmin)
        {
            return true;
        }

        return user.IsTeacher && user.TeacherId != null && subjectTeacherId == user.TeacherId;
    }
}
