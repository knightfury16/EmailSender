using System.Net.Mime;

namespace EmailSenderLib.Models;

/// <summary>
/// Represents an email attachment with its content and metadata.
/// </summary>
public class EmailAttachment : IDisposable
{
    private bool _disposed;
    private const int MaxAttachmentSize = 25 * 1024 * 1024; // 25MiB
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt", ".html", ".htm", ".jpg", ".jpeg", ".png", ".gif", ".pdf",
        ".doc", ".docx", ".xls", ".xlsx", ".zip", ".csv", ".rtf", ".odt",
        ".ods", ".odp", ".ppt", ".pptx"
    };

    public string FileName { get; }
    public Stream Content { get; }
    public string MimeType { get; }

    public bool IsInline { get; }
    public string? ContentId { get; }

    private EmailAttachment(
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

        if (!content.CanSeek)
        {
            throw new NotSupportedException("Cannot check size of a non-seekabel stream.");
        }

        if (content.Length > MaxAttachmentSize)
        {
            throw new InvalidOperationException($"Attachment size exceeded the max attachment size: {MaxAttachmentSize} bytes");
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

    //Factory Method
    public static EmailAttachment FromStream(
        Stream stream,
        string fileName,
        string? mimeType = null,
        bool isInline = false,
        string? contentId = null
    )
    {
        var resolvedMimeType = mimeType ?? GetMimeTypeFromFileName(fileName);
        return new EmailAttachment(stream, fileName, resolvedMimeType, isInline, contentId);
    }

    public static EmailAttachment FromByte(
        byte[] bytes,
        string fileName,
        string? mimeType = null,
        bool isInline = false,
        string? contentId = null
    )
    {
        var stream = new MemoryStream(bytes);
        return FromStream(stream, fileName, mimeType, isInline, contentId);
    }

    public static EmailAttachment FromFile(
        string filePath,
        string? mimeType = null,
        bool isInline = false,
        string? contentId = null
    )
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        var stream = File.OpenRead(filePath);
        var fileName = Path.GetFileName(filePath);
        var resolvedMimeType = mimeType ?? GetMimeTypeFromFileName(fileName);
        return FromStream(stream, fileName, resolvedMimeType, isInline, contentId);
    }

    //for internal use only
    internal System.Net.Mail.Attachment ToSystemMailAttachment()
    {
        var attachment = new System.Net.Mail.Attachment(Content, FileName, MimeType);

        if (IsInline)
        {
            attachment.ContentId = ContentId;
            if (attachment.ContentDisposition != null)
            {
                attachment.ContentDisposition.Inline = true;
                attachment.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
            }
        }
        return attachment;
    }

    private static string GetMimeTypeFromFileName(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        return ext switch
        {
            ".txt" => "text/plain",
            ".html" => "text/html",
            ".htm" => "text/html",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".zip" => "application/zip",
            _ => "application/octet-stream",
        };
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        Content.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
