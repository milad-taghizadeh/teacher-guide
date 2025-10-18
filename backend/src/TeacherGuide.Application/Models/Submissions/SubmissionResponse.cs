using TeacherGuide.Domain.Enums;

namespace TeacherGuide.Application.Models.Submissions;

public record SubmissionResponse(Guid SubmissionId, SubmissionStatus Status, DateTime SubmittedAt);
