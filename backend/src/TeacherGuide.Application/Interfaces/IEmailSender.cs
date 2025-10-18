namespace TeacherGuide.Application.Interfaces;

public interface IEmailSender
{
    Task SendInviteAsync(string to, string subject, string body, CancellationToken cancellationToken);
}
