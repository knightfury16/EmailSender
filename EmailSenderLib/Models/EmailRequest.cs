namespace EmailSenderLib.Models;

/// <summary>
/// Represents a request to send a standard email with text and HTML content.
/// </summary>
public sealed class EmailRequest : SendRequest
{
    private const int MaxContentLength = 1024 * 1024; // 1MB limit for content
    private string? _textContent;
    private string? _htmlContent;

    //If both Text and Html content is set, Html content will take precedence
    public string? TextContent
    {
        get => _textContent;
        set
        {
            if (value != null && value.Length > MaxContentLength)
            {
                throw new ArgumentException(
                    $"Text content cannot exceed {MaxContentLength} characters.",
                    nameof(value)
                );
            }
            _textContent = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }

    public string? HtmlContent
    {
        get => _htmlContent;
        set
        {
            if (value != null && value.Length > MaxContentLength)
            {
                throw new ArgumentException(
                    $"HTML content cannot exceed {MaxContentLength} characters.",
                    nameof(value)
                );
            }
            _htmlContent = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }

    public bool HasContent =>
        !string.IsNullOrWhiteSpace(_textContent) || !string.IsNullOrWhiteSpace(_htmlContent);

    internal EmailRequest()
        : base() { }

    public EmailRequest(ICollection<EmailAddress> to)
        : base(to) { }

    public EmailRequest(EmailAddress to)
        : base(to) { }

    public EmailRequest SetContent(string? textContent, string? htmlContent)
    {
        if (string.IsNullOrWhiteSpace(textContent) && string.IsNullOrWhiteSpace(htmlContent))
        {
            throw new ArgumentException("At least one of textContent or htmlContent must be provided.");
        }

        TextContent = textContent;
        HtmlContent = htmlContent;
        return this;
    }

    public override void Validate()
    {
        base.Validate();

        if (!HasContent)
        {
            throw new InvalidOperationException(
                "Email must have either textContent or htmlContent."
            );
        }

        if (_textContent != null && _textContent.Length > MaxContentLength)
        {
            throw new InvalidOperationException(
                $"Text content cannot exceed {MaxContentLength} characters."
            );
        }

        if (_htmlContent != null && _htmlContent.Length > MaxContentLength)
        {
            throw new InvalidOperationException(
                $"HTML content cannot exceed {MaxContentLength} characters."
            );
        }
    }
}
