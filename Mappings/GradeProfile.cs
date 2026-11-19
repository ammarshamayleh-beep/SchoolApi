using AutoMapper;
using SchoolApi.DTOs.Grades;
using SchoolApi.Models;

namespace SchoolApi.Mappings;

public class GradeProfile : Profile
{
    public GradeProfile()
    {
        // Entity -> Response DTO
        // SubjectCode comes from Subject.Code by itself (Flattening)
        CreateMap<Grade, GradeDto>()
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s => s.Student!.FirstName + " " + s.Student.LastName));

        // Request DTO -> Entity (POST)
        CreateMap<GradeRequestDto, Grade>(MemberList.Source);

        // Update DTO -> Entity (PUT: only the score changes)
        CreateMap<GradeUpdateDto, Grade>(MemberList.Source);
    }
}
