using System.Globalization;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TeacherGuide.Domain.Entities;
using TeacherGuide.Infrastructure;
using TeacherGuide.Infrastructure.Configuration;
using TeacherGuide.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "سامانه انتخاب استاد",
        Version = "v1"
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    var origins = builder.Configuration.GetSection("Frontend:CorsOrigins").Get<string[]>() ?? Array.Empty<string>();
    options.AddPolicy("Default", policy =>
    {
        if (origins.Length > 0)
        {
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        }
        else
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    });
});

var jwtSigningKey = builder.Configuration["Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(jwtSigningKey))
{
    throw new InvalidOperationException("Jwt signing key is not configured");
}
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

var supportedCultures = new[] { new CultureInfo("fa-IR") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("fa-IR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

app.UseRequestLocalization(localizationOptions);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "سامانه انتخاب استاد");
        options.DocumentTitle = "Teacher Guide";
    });
}

app.UseHttpsRedirection();
app.UseCors("Default");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await initializer.InitializeAsync(CancellationToken.None);
    var adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminSeedOptions>>().Value;
    if (!string.IsNullOrWhiteSpace(adminOptions.Username) && !string.IsNullOrWhiteSpace(adminOptions.Password))
    {
        var admin = new AdminUser
        {
            Id = Guid.NewGuid(),
            Username = adminOptions.Username,
            PasswordHash = BCrypt.HashPassword(adminOptions.Password),
            CreatedAt = DateTime.UtcNow
        };
        await initializer.SeedAdminAsync(admin, CancellationToken.None);
    }
}

app.Run();
