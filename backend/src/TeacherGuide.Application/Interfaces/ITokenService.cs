using TeacherGuide.Domain.Entities;

namespace TeacherGuide.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(AdminUser user);
}
