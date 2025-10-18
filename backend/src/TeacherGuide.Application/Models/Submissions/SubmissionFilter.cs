using TeacherGuide.Domain.Enums;

namespace TeacherGuide.Application.Models.Submissions;

public record SubmissionFilter(SubmissionStatus? Status, string? Search, DateTime? From, DateTime? To);
