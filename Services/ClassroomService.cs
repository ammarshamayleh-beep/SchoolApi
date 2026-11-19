using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Classrooms;
using SchoolApi.Extensions;
using SchoolApi.Models;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Services;

public class ClassroomService : IClassroomService
{
    private readonly SchoolContext _context;
    private readonly IMapper _mapper;

    public ClassroomService(SchoolContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<PagedResult<ClassroomDto>>> GetAllAsync(PaginationQuery query)
    {
        var page = await _context.Classrooms
            .OrderBy(c => c.Name)
            .ProjectTo<ClassroomDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(query);

        return BaseResponse<PagedResult<ClassroomDto>>.Success(page);
    }

    public async Task<BaseResponse<ClassroomDto>> GetByIdAsync(Guid id)
    {
        var classroom = await _context.Classrooms
            .Where(c => c.Id == id)
            .ProjectTo<ClassroomDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (classroom == null)
        {
            return BaseResponse<ClassroomDto>.Failure(StatusCodes.Status404NotFound, $"Classroom {id} was not found.");
        }

        return BaseResponse<ClassroomDto>.Success(classroom);
    }

    // Database trips: 1 (the INSERT)
    public async Task<BaseResponse<ClassroomDto>> CreateAsync(ClassroomRequestDto dto)
    {
        var classroom = _mapper.Map<Classroom>(dto);

        _context.Classrooms.Add(classroom);
        await _context.SaveChangesAsync();

        // A new classroom has no students yet, so we build the DTO in memory (no second query)
        var created = _mapper.Map<ClassroomDto>(classroom);

        return BaseResponse<ClassroomDto>.Success(created, StatusCodes.Status201Created);
    }

    // Database trips: 2 (load + UPDATE)
    public async Task<BaseResponse<bool>> UpdateAsync(Guid id, ClassroomRequestDto dto)
    {
        var classroom = await _context.Classrooms.FindAsync(id);

        if (classroom == null)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status404NotFound, $"Classroom {id} was not found.");
        }

        _mapper.Map(dto, classroom);
        await _context.SaveChangesAsync();

        return BaseResponse<bool>.Success(true);
    }

    // Database trips: 2 (one query for the classroom and its students check + UPDATE)
    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        // One query: the classroom itself, and whether it has students
        var found = await _context.Classrooms
            .Where(c => c.Id == id)
            .Select(c => new { Classroom = c, HasStudents = c.Students.Any() })
            .FirstOrDefaultAsync();

        if (found == null)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status404NotFound, $"Classroom {id} was not found.");
        }

        // Business rule: a classroom with students can't be deleted
        if (found.HasStudents)
        {
            return BaseResponse<bool>.Failure(StatusCodes.Status409Conflict,
                $"Classroom {id} has students. Move them to another classroom first.");
        }

        _context.Classrooms.Remove(found.Classroom);
        await _context.SaveChangesAsync();

        return BaseResponse<bool>.Success(true);
    }
}
