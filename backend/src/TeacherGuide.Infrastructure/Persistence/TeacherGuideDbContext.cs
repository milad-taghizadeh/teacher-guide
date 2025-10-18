using Microsoft.EntityFrameworkCore;
using TeacherGuide.Domain.Entities;
using TeacherGuide.Domain.Enums;

namespace TeacherGuide.Infrastructure.Persistence;

public class TeacherGuideDbContext : DbContext
{
    public TeacherGuideDbContext(DbContextOptions<TeacherGuideDbContext> options) : base(options)
    {
    }

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<StudentInvite> StudentInvites => Set<StudentInvite>();
    public DbSet<StudentSubmission> StudentSubmissions => Set<StudentSubmission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
        });

        modelBuilder.Entity<StudentInvite>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.StudentNumber).IsRequired();
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.HasMany(e => e.Submissions)
                .WithOne(s => s.Invite)
                .HasForeignKey(s => s.InviteId);
        });

        modelBuilder.Entity<StudentSubmission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StudentNumber).IsRequired();
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.TeacherName).IsRequired();
            entity.Property(e => e.TopicTitle).IsRequired();
            entity.Property(e => e.Status).HasDefaultValue(SubmissionStatus.Pending);
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
