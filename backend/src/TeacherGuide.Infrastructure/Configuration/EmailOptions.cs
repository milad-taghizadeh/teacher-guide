namespace TeacherGuide.Infrastructure.Configuration;

public class EmailOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string SenderAddress { get; set; } = string.Empty;
    public string SenderDisplayName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
