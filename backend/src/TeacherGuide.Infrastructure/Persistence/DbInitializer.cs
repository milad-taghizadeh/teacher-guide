using Microsoft.EntityFrameworkCore;
using TeacherGuide.Domain.Entities;

namespace TeacherGuide.Infrastructure.Persistence;

public class DbInitializer
{
    private readonly TeacherGuideDbContext _context;

    public DbInitializer(TeacherGuideDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await _context.Database.MigrateAsync(cancellationToken);
    }

    public async Task SeedAdminAsync(AdminUser admin, CancellationToken cancellationToken)
    {
        if (await _context.AdminUsers.AnyAsync(cancellationToken))
        {
            return;
        }

        _context.AdminUsers.Add(admin);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
