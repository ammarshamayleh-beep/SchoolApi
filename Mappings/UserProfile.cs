using AutoMapper;
using SchoolApi.DTOs.Auth;
using SchoolApi.Models;

namespace SchoolApi.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Entity -> Response DTO
        // UserDto has no PasswordHash, so the hash never leaves the API
        CreateMap<User, UserDto>();
    }
}
