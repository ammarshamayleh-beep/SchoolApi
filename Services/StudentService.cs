using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Students;
using SchoolApi.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SchoolApi.Models;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Services;

public class StudentService : IStudentService
{
    private readonly SchoolContext _context;
    private readonly IMapper _mapper;

    public StudentService(SchoolContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<PagedResult<StudentDto>>> GetAllAsync(StudentQuery q)
    {
        IQueryable<Student> query = _context.Students;

        // 1. Filtering: only the filters the client sent
        string? search = q.Search;
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                s.FirstName.Contains(search) ||
                s.LastName.Contains(search) ||
                s.Email.Contains(search));
        }

        if (q.ClassroomId != null)
        {
            Guid classroomId = q.ClassroomId.Value;
            query = query.Where(s => s.ClassroomId == classroomId);
        }

        // 2. Sorting
        if (q.SortBy?.ToLower() == "dob")
        {
            query = q.Desc ? query.OrderByDescending(s => s.DateOfBirth) : query.OrderBy(s => s.DateOfBirth);
        }
        else
        {
            query = q.Desc
                ? query.OrderByDescending(s => s.FirstName).ThenByDescending(s => s.LastName)
                : query.OrderBy(s => s.FirstName).ThenBy(s => s.LastName);
        }

        // 3. Pagination: one page, plus the total count
        var page = await query
            .ProjectTo<StudentDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(q);

        return BaseResponse<PagedResult<StudentDto>>.Success(page);
    }

    public async Task<BaseResponse<StudentDto>> GetByIdAsync(Guid id)
    {
        var student = await _context.Students
            .Where(s => s.Id == id)
            .ProjectTo<StudentDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (student == null)
        {
            return BaseResponse<StudentDto>.Failure(StatusCodes.Status404NotFound, $"Student {id} was not found.");
        }

        return BaseResponse<StudentDto>.Success(student);
    }

    // Database trips: 2 (the classroom check + the INSERT)
    public async Task<BaseResponse<StudentDto>> CreateAsync(StudentRequestDto dto)
    {
        // Rule 1: the classroom must exist.
        // We read its name in the same query, because the response needs it.
        var classroomName = await _context.Classrooms
            .Where(c => c.Id == dto.ClassroomId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync();

        if (classroomName == null)
        {
            return BaseResponse<StudentDto>.Failure(StatusCodes.Status400BadRequest, $"Classroom {dto.ClassroomId} was not found.");
        }

        var student = _mapper.Map<Student>(dto);

        _context.Students.Add(student);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            // Rule 2: the email is unique. The unique index checked it, so no AnyAsync before saving.
            return BaseResponse<StudentDto>.Failure(StatusCodes.Status409Conflict, $"Email {dto.Email} is already used.");
        }

        // Build the DTO in memory (no second query), with the classroom name we already have
        var created = _mapper.Map<StudentDto>(student);
        created.ClassroomName = classroomName;

        return BaseResponse<StudentDto>.Success(created, StatusCodes.Status201Created);
    }

    // Database trips: 2 (load + UPDATE), or 3 when the classroom changes
    public async Task<BaseResponse<bool>> UpdateAsync(Guid id, StudentRequestDto dto)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status404NotFound, $"Student {id} was not found.");
        }

        // Rule 1: a new classroom must exist. We only ask the database when the classroom changes.
        if (dto.ClassroomId != student.ClassroomId)
        {
            if (!await _context.Classrooms.AnyAsync(c => c.Id == dto.ClassroomId))
            {
                return BaseResponse<bool>.Failure(StatusCodes.Status400BadRequest, $"Classroom {dto.ClassroomId} was not found.");
            }
        }

        _mapper.Map(dto, student);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            // Rule 2: the email is unique (another student already has it)
            return BaseResponse<bool>.Failure(StatusCodes.Status409Conflict, $"Email {dto.Email} is already used.");
        }

        return BaseResponse<bool>.Success(true);
    }

    // Database trips: 2 (one query for the student, his grades and his account + the UPDATEs)
    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        // One query: the student, his grades and his login account
        var found = await _context.Students
            .Where(s => s.Id == id)
            .Select(s => new
            {
                Student = s,
                Grades = s.Grades.ToList(),
                Account = _context.Users.FirstOrDefault(u => u.StudentId == s.Id)
            })
            .FirstOrDefaultAsync();

        if (found == null)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status404NotFound, $"Student {id} was not found.");
        }

        // Soft Delete is an UPDATE, so the database Cascade never runs.
        // We delete his grades and his login account ourselves.
        if (found.Account != null)
        {
            _context.Users.Remove(found.Account);
        }

        _context.Grades.RemoveRange(found.Grades);
        _context.Students.Remove(found.Student);

        // SchoolContext turns every Remove into IsDeleted = true. All the UPDATEs go in one SaveChanges.
        await _context.SaveChangesAsync();

        return BaseResponse<bool>.Success(true);
    }
}
