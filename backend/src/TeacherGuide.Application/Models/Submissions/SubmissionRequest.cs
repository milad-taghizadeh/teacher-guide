namespace TeacherGuide.Application.Models.Submissions;

public record SubmissionRequest(
    string Token,
    string StudentNumber,
    string FirstName,
    string LastName,
    string TeacherName,
    string PhoneNumber,
    string Email,
    string TopicTitle,
    string Description
);
