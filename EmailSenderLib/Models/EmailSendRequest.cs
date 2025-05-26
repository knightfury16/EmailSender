namespace EmailSenderLib.Models;

/// <summary>
/// Represents a request to send a standard email with text and HTML content.
/// </summary>
public sealed class EmailSendRequest : SendRequest
{
    private string? _textContent;
    private string? _htmlContent;

    public string? TextContent
    {
        get => _textContent;
        set { _textContent = string.IsNullOrWhiteSpace(value) ? null : value.Trim(); }
    }

    public string? HtmlContent
    {
        get => _htmlContent;
        set { _htmlContent = string.IsNullOrWhiteSpace(value) ? null : value.Trim(); }
    }

    public bool HasContent =>
        !string.IsNullOrWhiteSpace(_textContent) || !string.IsNullOrWhiteSpace(_htmlContent);

    internal EmailSendRequest()
        : base() { }

    public EmailSendRequest(ICollection<EmailAddress> to)
        : base(to) { }

    public EmailSendRequest(EmailAddress to)
        : base(to) { }

    public EmailSendRequest SetContent(string? textContent, string? htmlContent)
    {
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
    }
}
