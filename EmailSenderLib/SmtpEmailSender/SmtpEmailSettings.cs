using EmailSenderLib.Models;

namespace EmailSenderLib.SmtpEmailSender;

public class SmtpEmailSettings : EmailSettings
{
    public string? SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public bool EnableSsl { get; set; }
}
