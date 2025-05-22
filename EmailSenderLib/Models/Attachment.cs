namespace EmailSenderLib.Models;

/// <summary>
/// Represents an email attachment with its content and metadata.
/// </summary>
public class Attachment
{
    /// <summary>
    /// Gets or sets the binary content of the attachment.
    /// </summary>
    public byte[]? Content { get; set; }

    /// <summary>
    /// Gets or sets the name of the attached file.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the attachment content.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the content identifier used for inline attachments.
    /// </summary>
    public string? ContentId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the attachment should be displayed inline in the email body.
    /// </summary>
    public bool IsInline { get; set; }

    /// <summary>
    /// Initializes a new instance of the Attachment class.
    /// </summary>
    public Attachment() { }

    /// <summary>
    /// Converts this attachment to a System.Net.Mail.Attachment object.
    /// </summary>
    /// <returns>A System.Net.Mail.Attachment object representing the same attachment.</returns>
    /// <exception cref="ArgumentNullException">Thrown when Content or FileName is null.</exception>
    public System.Net.Mail.Attachment ToSystemMailAttachment()
    {
        if (Content == null)
        {
            throw new ArgumentNullException(nameof(Content), "Attachment content cannot be null");
        }

        if (string.IsNullOrEmpty(FileName))
        {
            throw new ArgumentNullException(
                nameof(FileName),
                "Attachment filename cannot be null or empty"
            );
        }

        var memoryStream = new MemoryStream(Content);
        var attachment = new System.Net.Mail.Attachment(memoryStream, FileName);

        if (!string.IsNullOrEmpty(ContentType))
        {
            attachment.ContentType = new System.Net.Mime.ContentType(ContentType);
        }

        if (IsInline && !string.IsNullOrEmpty(ContentId))
        {
            attachment.ContentId = ContentId;
            if (attachment.ContentDisposition != null)
            {
                attachment.ContentDisposition.Inline = true;
            }
        }

        return attachment;
    }
}
