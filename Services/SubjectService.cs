using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Subjects;
using SchoolApi.Extensions;
using SchoolApi.Models;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Services;

public class SubjectService : ISubjectService
{
    private readonly SchoolContext _context;
    private readonly IMapper _mapper;

    public SubjectService(SchoolContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<PagedResult<SubjectDto>>> GetAllAsync(PaginationQuery query)
    {
        var page = await _context.Subjects
            .OrderBy(s => s.Name)
            .ProjectTo<SubjectDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(query);

        return BaseResponse<PagedResult<SubjectDto>>.Success(page);
    }

    public async Task<BaseResponse<SubjectDto>> GetByIdAsync(Guid id)
    {
        var subject = await _context.Subjects
            .Where(s => s.Id == id)
            .ProjectTo<SubjectDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (subject == null)
        {
            return BaseResponse<SubjectDto>.Failure(StatusCodes.Status404NotFound, $"Subject {id} was not found.");
        }

        return BaseResponse<SubjectDto>.Success(subject);
    }

    // Database trips: 2 (the teacher check + the INSERT), or 1 when there is no teacher
    public async Task<BaseResponse<SubjectDto>> CreateAsync(SubjectRequestDto dto)
    {
        // Rule 1: if there is a teacher, he must exist.
        // We read his name in the same query, because the response needs it.
        string? teacherName = null;

        if (dto.TeacherId != null)
        {
            teacherName = await FindTeacherNameAsync(dto.TeacherId.Value);

            if (teacherName == null)
            {
                return BaseResponse<SubjectDto>.Failure(StatusCodes.Status400BadRequest, $"Teacher {dto.TeacherId} was not found.");
            }
        }

        var subject = _mapper.Map<Subject>(dto);

        _context.Subjects.Add(subject);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            // Rule 2: the code is unique. The unique index checked it, so no AnyAsync before saving.
            return BaseResponse<SubjectDto>.Failure(StatusCodes.Status409Conflict, $"Subject code {dto.Code} is already used.");
        }

        // Build the DTO in memory (no second query), with the teacher name we already have
        var created = _mapper.Map<SubjectDto>(subject);
        created.TeacherName = teacherName;

        return BaseResponse<SubjectDto>.Success(created, StatusCodes.Status201Created);
    }

    // Database trips: 2 (load + UPDATE), or 3 when the teacher changes
    public async Task<BaseResponse<bool>> UpdateAsync(Guid id, SubjectRequestDto dto)
    {
        var subject = await _context.Subjects.FindAsync(id);

        if (subject == null)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status404NotFound, $"Subject {id} was not found.");
        }

        // Rule 1: a new teacher must exist. We only ask the database when the teacher changes.
        if (dto.TeacherId != null && dto.TeacherId != subject.TeacherId)
        {
            if (!await _context.Teachers.AnyAsync(t => t.Id == dto.TeacherId))
            {
                return BaseResponse<bool>.Failure(StatusCodes.Status400BadRequest, $"Teacher {dto.TeacherId} was not found.");
            }
        }

        _mapper.Map(dto, subject);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            // Rule 2: the code is unique (another subject already has it)
            return BaseResponse<bool>.Failure(StatusCodes.Status409Conflict, $"Subject code {dto.Code} is already used.");
        }

        return BaseResponse<bool>.Success(true);
    }

    // Database trips: 2 (one query for the subject and its grades check + UPDATE)
    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        // One query: the subject itself, and whether it has grades
        var found = await _context.Subjects
            .Where(s => s.Id == id)
            .Select(s => new { Subject = s, HasGrades = s.Grades.Any() })
            .FirstOrDefaultAsync();

        if (found == null)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status404NotFound, $"Subject {id} was not found.");
        }

        // Rule 3: a subject with grades can't be deleted
        if (found.HasGrades)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status409Conflict, $"Subject {id} has grades, so it can't be deleted.");
        }

        _context.Subjects.Remove(found.Subject);
        await _context.SaveChangesAsync();

        return BaseResponse<bool>.Success(true);
    }

    // The teacher's full name, or null when there is no such teacher
    private async Task<string?> FindTeacherNameAsync(Guid teacherId)
    {
        return await _context.Teachers
            .Where(t => t.Id == teacherId)
            .Select(t => t.FirstName + " " + t.LastName)
            .FirstOrDefaultAsync();
    }
}
