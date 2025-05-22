namespace EmailSenderLib.Models;
/// <summary>
/// Represents a request to send a standard email with text and HTML content.
/// </summary>
public class EmailSendRequest : SendRequest
{
    /// <summary>
    /// Gets or sets the plain text content of the email.
    /// </summary>
    public string? TextContent { get; set; }

    /// <summary>
    /// Gets or sets the HTML content of the email.
    /// </summary>
    public string? HtmlContent { get; set; }

    /// <summary>
    /// Initializes a new instance of the EmailSendRequest class with multiple recipients.
    /// </summary>
    /// <param name="to">The collection of primary recipients.</param>
    public EmailSendRequest(ICollection<EmailAddress> to)
        : base(to) { }

    /// <summary>
    /// Initializes a new instance of the EmailSendRequest class with a single recipient.
    /// </summary>
    /// <param name="to">The primary recipient.</param>
    public EmailSendRequest(EmailAddress to)
        : base(to) { }

    /// <summary>
    /// Initializes a new instance of the EmailSendRequest class with default values.
    /// </summary>
    public EmailSendRequest()
        : base() { }
}
