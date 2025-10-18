using Microsoft.EntityFrameworkCore;
using TeacherGuide.Application.Interfaces;
using TeacherGuide.Application.Models.Submissions;
using TeacherGuide.Domain.Entities;
using TeacherGuide.Domain.Enums;
using TeacherGuide.Infrastructure.Persistence;

namespace TeacherGuide.Infrastructure.Services;

public class SubmissionService : ISubmissionService
{
    private readonly TeacherGuideDbContext _context;
    private readonly IClock _clock;

    public SubmissionService(TeacherGuideDbContext context, IClock clock)
    {
        _context = context;
        _clock = clock;
    }

    public async Task<SubmissionResponse> CreateSubmissionAsync(SubmissionRequest request, CancellationToken cancellationToken)
    {
        var invite = await _context.StudentInvites.SingleOrDefaultAsync(x => x.Token == request.Token, cancellationToken);
        if (invite == null || invite.IsUsed || invite.ExpiresAt < _clock.UtcNow)
        {
            throw new InvalidOperationException("Invalid invite");
        }

        var submission = new StudentSubmission
        {
            Id = Guid.NewGuid(),
            InviteId = invite.Id,
            StudentNumber = request.StudentNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            TeacherName = request.TeacherName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            TopicTitle = request.TopicTitle,
            Description = request.Description,
            Status = SubmissionStatus.Pending,
            SubmittedAt = _clock.UtcNow
        };

        invite.IsUsed = true;
        _context.StudentSubmissions.Add(submission);
        await _context.SaveChangesAsync(cancellationToken);

        return new SubmissionResponse(submission.Id, submission.Status, submission.SubmittedAt);
    }

    public async Task<IReadOnlyList<SubmissionSummary>> GetSubmissionsAsync(SubmissionFilter filter, CancellationToken cancellationToken)
    {
        var query = _context.StudentSubmissions.AsNoTracking().AsQueryable();
        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var value = filter.Search.Trim();
            query = query.Where(x => x.StudentNumber.Contains(value) || (x.FirstName + " " + x.LastName).Contains(value) || x.TeacherName.Contains(value));
        }
        if (filter.From.HasValue)
        {
            query = query.Where(x => x.SubmittedAt >= filter.From.Value);
        }
        if (filter.To.HasValue)
        {
            query = query.Where(x => x.SubmittedAt <= filter.To.Value);
        }
        var items = await query
            .OrderByDescending(x => x.SubmittedAt)
            .Select(x => new SubmissionSummary(
                x.Id,
                x.StudentNumber,
                x.FirstName + " " + x.LastName,
                x.TeacherName,
                x.TopicTitle,
                x.Status,
                x.SubmittedAt))
            .ToListAsync(cancellationToken);
        return items;
    }

    public async Task UpdateStatusAsync(Guid submissionId, SubmissionStatusUpdate request, CancellationToken cancellationToken)
    {
        var submission = await _context.StudentSubmissions.SingleOrDefaultAsync(x => x.Id == submissionId, cancellationToken);
        if (submission == null)
        {
            throw new KeyNotFoundException();
        }
        submission.Status = request.Status;
        submission.UpdatedAt = _clock.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
