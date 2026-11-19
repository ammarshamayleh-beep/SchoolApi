using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Teachers;
using SchoolApi.Extensions;
using SchoolApi.Models;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Services;

public class TeacherService : ITeacherService
{
    private readonly SchoolContext _context;
    private readonly IMapper _mapper;

    public TeacherService(SchoolContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<PagedResult<TeacherDto>>> GetAllAsync(PaginationQuery query)
    {
        var page = await _context.Teachers
            .OrderBy(t => t.FirstName)
            .ThenBy(t => t.LastName)
            .ProjectTo<TeacherDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(query);

        return BaseResponse<PagedResult<TeacherDto>>.Success(page);
    }

    public async Task<BaseResponse<TeacherDto>> GetByIdAsync(Guid id)
    {
        var teacher = await _context.Teachers
            .Where(t => t.Id == id)
            .ProjectTo<TeacherDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (teacher == null)
        {
            return BaseResponse<TeacherDto>.Failure(StatusCodes.Status404NotFound, $"Teacher {id} was not found.");
        }

        return BaseResponse<TeacherDto>.Success(teacher);
    }

    // Database trips: 1 (the INSERT)
    public async Task<BaseResponse<TeacherDto>> CreateAsync(TeacherRequestDto dto)
    {
        var teacher = _mapper.Map<Teacher>(dto);

        _context.Teachers.Add(teacher);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            // Rule: the email is unique. The unique index checked it, so no AnyAsync before saving.
            return BaseResponse<TeacherDto>.Failure(StatusCodes.Status409Conflict, $"Email {dto.Email} is already used.");
        }

        // A new teacher has no subjects yet, so we build the DTO in memory (no second query)
        var created = _mapper.Map<TeacherDto>(teacher);

        return BaseResponse<TeacherDto>.Success(created, StatusCodes.Status201Created);
    }

    // Database trips: 2 (load + UPDATE)
    public async Task<BaseResponse<bool>> UpdateAsync(Guid id, TeacherRequestDto dto)
    {
        var teacher = await _context.Teachers.FindAsync(id);

        if (teacher == null)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status404NotFound, $"Teacher {id} was not found.");
        }

        _mapper.Map(dto, teacher);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            // Rule: the email is unique (another teacher already has it)
            return BaseResponse<bool>.Failure(StatusCodes.Status409Conflict, $"Email {dto.Email} is already used.");
        }

        return BaseResponse<bool>.Success(true);
    }

    // Database trips: 2 (one query for the teacher, his subjects and his account + the UPDATEs)
    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        // One query: the teacher, his subjects and his login account
        var found = await _context.Teachers
            .Where(t => t.Id == id)
            .Select(t => new
            {
                Teacher = t,
                Subjects = t.Subjects.ToList(),
                Account = _context.Users.FirstOrDefault(u => u.TeacherId == t.Id)
            })
            .FirstOrDefaultAsync();

        if (found == null)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status404NotFound, $"Teacher {id} was not found.");
        }

        // Soft Delete is an UPDATE, so the database SetNull and Cascade never run.
        // We do their work ourselves:
        // 1. His subjects stay, without a teacher
        foreach (var subject in found.Subjects)
        {
            subject.TeacherId = null;
        }

        // 2. His login account is deleted
        if (found.Account != null)
        {
            _context.Users.Remove(found.Account);
        }

        _context.Teachers.Remove(found.Teacher);

        // SchoolContext turns every Remove into IsDeleted = true. All the UPDATEs go in one SaveChanges.
        await _context.SaveChangesAsync();

        return BaseResponse<bool>.Success(true);
    }
}
