using EmailSenderLib.Models;

namespace EmailSenderLib.SmtpEmailSender;

public sealed class SmtpEmailSettings : EmailSettings
{
    public string? SmtpServer { get; set; }
    public int SmtpPort { get; set; } = 587; //default smtp port
    public bool EnableSsl { get; set; }
}
