using AutoMapper;
using SchoolApi.DTOs.Teachers;
using SchoolApi.Models;

namespace SchoolApi.Mappings;

public class TeacherProfile : Profile
{
    public TeacherProfile()
    {
        // Entity -> Response DTO
        CreateMap<Teacher, TeacherDto>()
            .ForMember(d => d.SubjectCount, opt => opt.MapFrom(s => s.Subjects.Count));

        // Request DTO -> Entity (POST and PUT)
        CreateMap<TeacherRequestDto, Teacher>(MemberList.Source);
    }
}
