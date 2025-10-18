using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using TeacherGuide.Application.Interfaces;
using TeacherGuide.Application.Models.Authentication;
using TeacherGuide.Infrastructure.Persistence;

namespace TeacherGuide.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly TeacherGuideDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthenticationService(TeacherGuideDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.AdminUsers.SingleOrDefaultAsync(x => x.Username == request.Username, cancellationToken);
        if (user == null || !BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException();
        }

        var token = _tokenService.CreateToken(user);
        return new LoginResponse(token, user.Username);
    }
}
