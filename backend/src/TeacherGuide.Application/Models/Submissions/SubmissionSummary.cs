using TeacherGuide.Domain.Enums;

namespace TeacherGuide.Application.Models.Submissions;

public record SubmissionSummary(
    Guid SubmissionId,
    string StudentNumber,
    string FullName,
    string TeacherName,
    string TopicTitle,
    SubmissionStatus Status,
    DateTime SubmittedAt
);
