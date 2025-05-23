namespace EmailSenderLib.OldModels;


public class SendRequest
{
    /// <summary>
    /// Gets or sets the sender's email address. This is nullable as it may be set from environment variables in the implementation.
    /// </summary>
    public EmailAddress? From { get; set; }

    /// <summary>
    /// Gets or sets the collection of primary recipients' email addresses.
    /// </summary>
    public ICollection<EmailAddress> To { get; set; } = new List<EmailAddress>();

    /// <summary>
    /// Gets or sets the collection of carbon copy (CC) recipients' email addresses.
    /// </summary>
    public ICollection<EmailAddress> Cc { get; set; } = new List<EmailAddress>();

    /// <summary>
    /// Gets or sets the collection of blind carbon copy (BCC) recipients' email addresses.
    /// </summary>
    public ICollection<EmailAddress> Bcc { get; set; } = new List<EmailAddress>();

    /// <summary>
    /// Gets or sets the subject line of the email.
    /// </summary>
    public required string Subject { get; set; }

    /// <summary>
    /// Gets or sets the collection of attachments to be included with the email.
    /// </summary>
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    /// <summary>
    /// Gets or sets the priority level of the email.
    /// </summary>
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;

    /// <summary>
    /// Gets or sets the custom headers to be included with the email.
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Initializes a new instance of the SendRequest class with default values.
    /// The To, Cc, Bcc, and Attachments collections will be initialized as empty collections,
    /// and Priority will be set to Normal.
    /// </summary>
    public SendRequest() { }

    /// <summary>
    /// Initializes a new instance of the SendRequest class with a collection of recipients.
    /// </summary>
    /// <param name="to">The collection of primary recipients.</param>
    /// <exception cref="ArgumentNullException">Thrown when the recipients collection is null.</exception>
    public SendRequest(ICollection<EmailAddress> to)
    {
        ArgumentNullException.ThrowIfNull(to);

        foreach (var recipient in to)
        {
            To.Add(recipient);
        }
    }

    /// <summary>
    /// Initializes a new instance of the SendRequest class with a single recipient.
    /// </summary>
    /// <param name="to">The primary recipient.</param>
    /// <exception cref="ArgumentNullException">Thrown when the recipient is null.</exception>
    public SendRequest(EmailAddress to)
    {
        ArgumentNullException.ThrowIfNull(to);
        To.Add(to);
    }

    /// <summary>
    /// Validates the email request. This method should be overridden in derived classes to add specific validation logic.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the validation fails.</exception>
    public virtual void Validate()
    {
        if (!To.Any())
            throw new InvalidOperationException("At least one recipient required.");
        if (string.IsNullOrEmpty(Subject))
            throw new InvalidOperationException("Subject is required.");
    }
