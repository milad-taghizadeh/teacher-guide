using TeacherGuide.Application.Models.Authentication;

namespace TeacherGuide.Application.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
