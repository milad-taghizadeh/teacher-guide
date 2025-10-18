using TeacherGuide.Application.Models.Invites;

namespace TeacherGuide.Application.Interfaces;

public interface IInviteService
{
    Task<InviteResponse> CreateInviteAsync(InviteRequest request, CancellationToken cancellationToken);
    Task<InviteDetails?> GetInviteAsync(string token, CancellationToken cancellationToken);
    Task MarkInviteAsUsedAsync(Guid inviteId, CancellationToken cancellationToken);
}
