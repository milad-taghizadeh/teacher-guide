using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeacherGuide.Application.Interfaces;
using TeacherGuide.Infrastructure.Configuration;
using TeacherGuide.Infrastructure.Persistence;
using TeacherGuide.Infrastructure.Services;

namespace TeacherGuide.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<EmailOptions>(configuration.GetSection("Email"));
        services.Configure<FrontendOptions>(configuration.GetSection("Frontend"));
        services.Configure<AdminSeedOptions>(configuration.GetSection("Admin"));

        services.AddDbContext<TeacherGuideDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Database"));
        });

        services.AddScoped<DbInitializer>();
        services.AddScoped<IClock, SystemClock>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IInviteService, InviteService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }
}
