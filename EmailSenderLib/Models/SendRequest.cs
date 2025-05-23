namespace EmailSenderLib.Models;

/// <summary>
/// Base class for all email sending requests, containing common properties and validation logic.
/// </summary>
public abstract class SendRequest
{
    private const int MaxSubjectLength = 998; //RFC 5322 limit
    private const int MaxRecipientPerType = 100;

    private readonly List<EmailAddress> _to = new();
    private readonly List<EmailAddress> _cc = new();
    private readonly List<EmailAddress> _bcc = new();
    private readonly List<Attachment> _attachments = new();
    private readonly Dictionary<string, string> _headers = new();

    public EmailAddress? From { get; set; }

    public ICollection<EmailAddress> To => _to;
    public ICollection<EmailAddress> Cc => _cc;
    public ICollection<EmailAddress> Bcc => _bcc;

    private string _subject = String.Empty;

    public required string Subject
    {
        get => _subject;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Subject can not be null or empty.", nameof(value));
            }

            if (_subject.Length > MaxSubjectLength)
            {
                throw new ArgumentException(
                    $"Subject can not exceed {MaxSubjectLength} characters.",
                    nameof(value)
                );
            }

            _subject = value.Trim();
        }
    }

    public ICollection<Attachment> Attachments => _attachments;

    public EmailPriority Priority { get; set; } = EmailPriority.Normal;
    public Dictionary<string, string> Headers => _headers;
    public int TotalRecipientsCount => _to.Count + _cc.Count + _bcc.Count;

    protected SendRequest() { }
}
