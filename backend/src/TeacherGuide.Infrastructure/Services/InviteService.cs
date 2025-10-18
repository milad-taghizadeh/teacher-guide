using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeacherGuide.Application.Interfaces;
using TeacherGuide.Application.Models.Invites;
using TeacherGuide.Domain.Entities;
using TeacherGuide.Infrastructure.Configuration;
using TeacherGuide.Infrastructure.Persistence;

namespace TeacherGuide.Infrastructure.Services;

public class InviteService : IInviteService
{
    private readonly TeacherGuideDbContext _context;
    private readonly IClock _clock;
    private readonly IEmailSender _emailSender;
    private readonly FrontendOptions _frontendOptions;

    public InviteService(TeacherGuideDbContext context, IClock clock, IEmailSender emailSender, IOptions<FrontendOptions> frontendOptions)
    {
        _context = context;
        _clock = clock;
        _emailSender = emailSender;
        _frontendOptions = frontendOptions.Value;
    }

    public async Task<InviteResponse> CreateInviteAsync(InviteRequest request, CancellationToken cancellationToken)
    {
        var token = Convert.ToHexString(Guid.NewGuid().ToByteArray()).ToLowerInvariant();
        var invite = new StudentInvite
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            StudentNumber = request.StudentNumber,
            FirstName = request.FirstName?.Trim(),
            LastName = request.LastName?.Trim(),
            Token = token,
            ExpiresAt = _clock.UtcNow.AddDays(3),
            CreatedAt = _clock.UtcNow,
            IsUsed = false
        };
        _context.StudentInvites.Add(invite);
        await _context.SaveChangesAsync(cancellationToken);

        var link = $"{_frontendOptions.BaseUrl.TrimEnd('/')}/form/{invite.Token}";
        var subject = "فرصت تکمیل فرم راهنمایی";
        var greetingName = string.Join(" ", new[] { invite.FirstName, invite.LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));
        var bodyPrefix = string.IsNullOrWhiteSpace(greetingName) ? "دانشجوی گرامی" : $"{greetingName} عزیز";
        var body = $"{bodyPrefix}، برای تکمیل اطلاعات راهنمایی از لینک زیر استفاده کنید:\n{link}";
        await _emailSender.SendInviteAsync(invite.Email, subject, body, cancellationToken);

        return new InviteResponse(invite.Id, invite.Token, invite.ExpiresAt);
    }

    public async Task<InviteDetails?> GetInviteAsync(string token, CancellationToken cancellationToken)
    {
        var invite = await _context.StudentInvites.AsNoTracking().SingleOrDefaultAsync(x => x.Token == token, cancellationToken);
        if (invite == null)
        {
            return null;
        }
        return new InviteDetails(invite.Id, invite.Email, invite.StudentNumber, invite.FirstName, invite.LastName, invite.ExpiresAt, invite.IsUsed);
    }

    public async Task MarkInviteAsUsedAsync(Guid inviteId, CancellationToken cancellationToken)
    {
        var invite = await _context.StudentInvites.SingleOrDefaultAsync(x => x.Id == inviteId, cancellationToken);
        if (invite == null)
        {
            return;
        }
        invite.IsUsed = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

