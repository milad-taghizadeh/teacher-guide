namespace TeacherGuide.Application.Models.Invites;

public record InviteResponse(Guid InviteId, string Token, DateTime ExpiresAt);
