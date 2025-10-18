using TeacherGuide.Domain.Enums;

namespace TeacherGuide.Domain.Entities;

public class StudentSubmission
{
    public Guid Id { get; set; }
    public Guid InviteId { get; set; }
    public StudentInvite? Invite { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TopicTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
