namespace EmailSenderLib.Models;

/// <summary>
/// Base class for all email sending requests, containing common properties and validation logic.
/// </summary>
public abstract class SendRequest
{
    private const int MaxSubjectLength = 998; //RFC 5322 limit
    private const int MaxRecipientPerType = 100;
    private const string ToType = "TO";
    private const string BccType = "BCC";
    private const string CcType = "CC";

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

    protected SendRequest(ICollection<EmailAddress> to)
    {
        ArgumentNullException.ThrowIfNull(to);
        AddRecipients(_to, to.ToArray(), ToType);
    }

    protected SendRequest(EmailAddress to)
        : this(new List<EmailAddress> { to }) { }

    //Include validatin and duplication check.
    //All add to collection should go through this method.
    private void AddRecipients(
        ICollection<EmailAddress> collection,
        EmailAddress[] recipients,
        string type
    )
    {
        foreach (var recipient in recipients)
        {
            if (recipient == null)
                continue;

            if (collection.Count > MaxRecipientPerType)
            {
                throw new InvalidOperationException(
                    $"Cannot add more than {MaxRecipientPerType} {type} recipients."
                );
            }

            //duplication check
            if (!collection.Contains(recipient))
            {
                collection.Add(recipient);
            }
        }
    }

    public void AddTo(params EmailAddress[] recipients)
    {
        AddRecipients(_to, recipients, ToType);
    }

    public void AddBcc(params EmailAddress[] recipients)
    {
        AddRecipients(_bcc, recipients, BccType);
    }

    public void AddCc(params EmailAddress[] recipients)
    {
        AddRecipients(_cc, recipients, CcType);
    }
}
