namespace EmailSenderLib.Models;
/// <summary>
/// Represents a request to send a templated email using a predefined template.
/// </summary>
public class TemplatedEmailRequest : SendRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the template to be used.
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content to be used for template variable substitution.
    /// </summary>
    public object TemplateContent { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the TemplatedEmailRequest class with multiple recipients.
    /// </summary>
    /// <param name="to">The collection of primary recipients.</param>
    public TemplatedEmailRequest(ICollection<EmailAddress> to)
        : base(to) { }

    /// <summary>
    /// Initializes a new instance of the TemplatedEmailRequest class with a single recipient.
    /// </summary>
    /// <param name="to">The primary recipient.</param>
    public TemplatedEmailRequest(EmailAddress to)
        : base(to) { }

    /// <summary>
    /// Initializes a new instance of the TemplatedEmailRequest class with default values.
    /// </summary>
    public TemplatedEmailRequest()
        : base() { }
}
