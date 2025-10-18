namespace TeacherGuide.Domain.Entities;

public class StudentInvite
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<StudentSubmission> Submissions { get; set; } = new List<StudentSubmission>();
}
