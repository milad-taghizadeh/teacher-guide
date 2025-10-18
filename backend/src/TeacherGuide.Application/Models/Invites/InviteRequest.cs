namespace TeacherGuide.Application.Models.Invites;

public record InviteRequest(string Email, string StudentNumber, string? FirstName, string? LastName);
