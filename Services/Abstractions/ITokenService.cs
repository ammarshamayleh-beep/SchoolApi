using SchoolApi.Models;

namespace SchoolApi.Services.Abstractions;

public interface ITokenService
{
    string CreateToken(User user);
}
