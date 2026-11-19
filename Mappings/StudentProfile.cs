using AutoMapper;
using SchoolApi.DTOs.Students;
using SchoolApi.Models;

namespace SchoolApi.Mappings;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        // Entity -> Response DTO
        // ClassroomName needs no rule: AutoMapper reads it from Classroom.Name (Flattening)
        CreateMap<Student, StudentDto>();

        // Request DTO -> Entity (POST and PUT)
        CreateMap<StudentRequestDto, Student>(MemberList.Source);
    }
}
