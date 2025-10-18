namespace TeacherGuide.Application.Models.Invites;

public record InviteDetails(Guid InviteId, string Email, string StudentNumber, string? FirstName, string? LastName, DateTime ExpiresAt, bool IsUsed);
