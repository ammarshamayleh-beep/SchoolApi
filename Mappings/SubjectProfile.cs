using AutoMapper;
using SchoolApi.DTOs.Subjects;
using SchoolApi.Models;

namespace SchoolApi.Mappings;

public class SubjectProfile : Profile
{
    public SubjectProfile()
    {
        // Entity -> Response DTO
        // A subject can have no teacher, so TeacherName can be null
        CreateMap<Subject, SubjectDto>()
            .ForMember(d => d.TeacherName, opt => opt.MapFrom(s =>
                s.Teacher != null ? s.Teacher.FirstName + " " + s.Teacher.LastName : null));

        // Request DTO -> Entity (POST and PUT)
        CreateMap<SubjectRequestDto, Subject>(MemberList.Source);
    }
}
