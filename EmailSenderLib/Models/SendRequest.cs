namespace EmailSenderLib.Models;

/// <summary>
/// Base class for all email sending requests, containing common properties and validation logic.
/// </summary>
public abstract class SendRequest : IDisposable
{
    private bool _disposed;
    private const int MaxSubjectLength = 998; //RFC 5322 limit
    private const int MaxRecipientPerType = 100;
    private const int MaxHeaderNameLength = 76; // RFC 5322 limit
    private const string ToType = "TO";
    private const string BccType = "BCC";
    private const string CcType = "CC";
    public static readonly string MessageIdKey = "MessageId";

    private readonly List<EmailAddress> _to = new();
    private readonly List<EmailAddress> _cc = new();
    private readonly List<EmailAddress> _bcc = new();
    private readonly List<EmailAttachment> _emailAttachments = new();
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

    public ICollection<EmailAttachment> EmailAttachments => _emailAttachments;

    public EmailPriority Priority { get; set; } = EmailPriority.Normal;
    public string MessageId { get; }
    public Dictionary<string, string> Headers => _headers;
    public int TotalRecipientsCount => _to.Count + _cc.Count + _bcc.Count;

    internal SendRequest()
    {
        MessageId = GenerateMessageId();
        _headers[MessageIdKey] = MessageId; // include it in the headers. Can use it later in Sytem.Net.Mail.MailMessage. MailMessage dont have MessgaId prop
    }

    protected SendRequest(ICollection<EmailAddress> to) : this()
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


    private string GenerateMessageId()
    {
        return Guid.NewGuid().ToString();
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

    public void AddHeader(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Header name cannot be null or empty.", nameof(name));
        }

        if (name.Length > MaxHeaderNameLength)
        {
            throw new ArgumentException(
                $"Header name too long {MaxHeaderNameLength}",
                nameof(name)
            );
        }

        _headers[name.Trim()] = value?.Trim() ?? string.Empty;
    }

    public void AddHeaders(Dictionary<string, string> headers)
    {
        ArgumentNullException.ThrowIfNull(headers);

        foreach (var header in headers)
        {
            AddHeader(header.Key, header.Value);
        }
    }

    public void AddAttachment(EmailAttachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);
        _emailAttachments.Add(attachment);
    }

    public void AddAttachment(List<EmailAttachment> attachments)
    {
        ArgumentNullException.ThrowIfNull(attachments);

        foreach (var attachment in attachments)
        {
            if (attachment == null) continue;
            _emailAttachments.Add(attachment);
        }
    }

    public virtual void Validate()
    {
        if (TotalRecipientsCount == 0)
        {
            throw new InvalidOperationException("At least one recipient is required.");
        }

        if (string.IsNullOrWhiteSpace(Subject))
        {
            throw new InvalidOperationException("Subject is required.");
        }

        // Validate no duplicate recipients across different types
        var allRecipients = To.Concat(Cc).Concat(Bcc).ToList();
        var duplicates = allRecipients
            .GroupBy(r => r.Address.ToLowerInvariant())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        if (duplicates.Any())
        {
            throw new InvalidOperationException(
                $"Duplicate recipients found: {string.Join(", ", duplicates)}"
            );
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        foreach (var attachment in EmailAttachments)
        {
            attachment?.Dispose();
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
