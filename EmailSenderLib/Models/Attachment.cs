namespace EmailSenderLib.Models;

/// <summary>
/// Represents an email attachment with its content and metadata.
/// </summary>
public class Attachment : IDisposable
{
    private bool _disposed;

    public string FileName { get; }
    public Stream Content { get; }
    public string MimeType { get; }

    public bool IsInline { get; }
    public string? ContentId { get; }

    private Attachment(
        Stream content,
        string fileName,
        string mimeType,
        bool isInline = false,
        string? contentId = null
    )
    {
        if (content == null || !content.CanRead)
        {
            throw new ArgumentException("Content stream must be readable.", nameof(content));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));
        }

        Content = content;
        FileName = fileName;
        MimeType = string.IsNullOrWhiteSpace(mimeType)
            ? "application/octet-stream"
            : mimeType.Trim();
        IsInline = isInline;

        if (isInline)
        {
            if (string.IsNullOrWhiteSpace(contentId))
            {
                throw new ArgumentException(
                    "Content Id must be provided for inline attachment.",
                    nameof(contentId)
                );
            }
            ContentId = contentId;
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
