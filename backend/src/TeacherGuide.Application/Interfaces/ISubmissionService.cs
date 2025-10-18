using TeacherGuide.Application.Models.Submissions;

namespace TeacherGuide.Application.Interfaces;

public interface ISubmissionService
{
    Task<SubmissionResponse> CreateSubmissionAsync(SubmissionRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<SubmissionSummary>> GetSubmissionsAsync(SubmissionFilter filter, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid submissionId, SubmissionStatusUpdate request, CancellationToken cancellationToken);
}
