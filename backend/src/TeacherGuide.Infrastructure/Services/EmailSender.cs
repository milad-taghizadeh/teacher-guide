using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using TeacherGuide.Application.Interfaces;
using TeacherGuide.Infrastructure.Configuration;

namespace TeacherGuide.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private readonly EmailOptions _options;

    public EmailSender(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendInviteAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.SenderDisplayName, _options.SenderAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, _options.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, cancellationToken);
        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
        }
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
