namespace EmailSenderLib.Models;

public abstract class EmailSettings
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? SenderEmail { get; set; }
    public string? SenderName { get; set; }
    public int TimeoutMilliseconds { get; set; } = 100000;
}
