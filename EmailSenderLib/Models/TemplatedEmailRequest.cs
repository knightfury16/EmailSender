namespace EmailSenderLib.Models;

/// <summary>
/// Represents a request to send a templated email using a predefined template.
/// </summary>
public sealed class TemplatedEmailRequest : SendRequest
{
    private string _templateId = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the template to be used.
    /// </summary>
    public string TemplateId
    {
        get => _templateId;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(
                    "TemplateId cannot be null or empty.",
                    nameof(value)
                );
            }

            _templateId = value.Trim();
        }
    }

    /// <summary>
    /// Gets or sets the content to be used for template variable substitution.
    /// </summary>
    public object? TemplateContent { get; set; }

    public bool HasTemplateContent => TemplateContent != null;

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
    internal TemplatedEmailRequest()
        : base() { }

    public TemplatedEmailRequest SetTemplate(string templateId, object? templateContent = null)
    {
        TemplateId = templateId;
        TemplateContent = templateContent;
        return this;
    }

    public override void Validate()
    {
        base.Validate();
    }
}
