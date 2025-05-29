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
            ValidateContentLength(value, ContentType.Text);
            _textContent = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }

    public string? HtmlContent
    {
        get => _htmlContent;
        set
        {
            ValidateContentLength(value, ContentType.Html);
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

    public EmailRequest SetTextContent(string? textContent)
    {
        TextContent = textContent; // this trigger the set where the validation takes place
        return this;
    }

    public EmailRequest SetHtmlContent(string? htmlContent)
    {
        HtmlContent = htmlContent; // this trigger the set where the validation takes place
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

        ValidateContentLength(_textContent, ContentType.Text);
        ValidateContentLength(_htmlContent, ContentType.Html);
    }

    public static void ValidateContentLength(string? content, string contentType)
    {
        if (content != null && content.Length > MaxContentLength)
        {
            throw new ArgumentException(
                $"{contentType} content cannot exceed {MaxContentLength} characters.",
                nameof(content)
            );
        }
    }
}

internal class ContentType
{
    public static readonly string Html = "Html";
    public static readonly string Text = "Text";
}
