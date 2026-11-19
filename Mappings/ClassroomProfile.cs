using AutoMapper;
using SchoolApi.DTOs.Classrooms;
using SchoolApi.Models;

namespace SchoolApi.Mappings;

public class ClassroomProfile : Profile
{
    public ClassroomProfile()
    {
        // Entity -> Response DTO
        // Id, Name, Year: same names, so AutoMapper copies them by itself
        CreateMap<Classroom, ClassroomDto>()
            .ForMember(d => d.StudentCount, opt => opt.MapFrom(s => s.Students.Count));

        // Request DTO -> Entity (POST and PUT)
        // MemberList.Source: check that every property of the DTO is used
        CreateMap<ClassroomRequestDto, Classroom>(MemberList.Source);
    }
}
