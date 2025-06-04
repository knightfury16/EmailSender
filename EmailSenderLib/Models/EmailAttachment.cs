using System.Net.Mime;

namespace EmailSenderLib.Models;

/// <summary>
/// Represents an email attachment with its content and metadata.
/// </summary>
public class EmailAttachment : IDisposable
{
    private bool _disposed;
    private Stream _content = default!;
    private const int MaxAttachmentSize = 25 * 1024 * 1024; // 25MiB
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt", ".html", ".htm", ".jpg", ".jpeg", ".png", ".gif", ".pdf",
        ".doc", ".docx", ".xls", ".xlsx", ".zip", ".csv", ".rtf", ".odt",
        ".ods", ".odp", ".ppt", ".pptx"
    };

    private static readonly HashSet<string> DangerousExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".dll", ".bat", ".cmd", ".ps1", ".vbs", ".js", ".jar",
        ".msi", ".reg", ".scr", ".pif", ".com", ".sys", ".drv"
    };

    public string FileName { get; }
    public Stream Content
    {
        get
        {
            ThrowIfDisposed();
            _content.Seek(0, SeekOrigin.Begin);
            return _content;
        }
        private set
        {
            _content = value;
        }
    }
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
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content), "Content stream cannot be null.");
        }

        if (!content.CanRead)
        {
            throw new ArgumentException("Content stream must be readable.", nameof(content));
        }

        if (!content.CanSeek)
        {
            throw new NotSupportedException("Content stream must be seekable to check size.");
        }

        ValidateFileName(fileName);
        ValidateFileSize(content);

        // Reset stream position after size check
        content.Position = 0;

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
                    "Content ID must be provided for inline attachment.",
                    nameof(contentId)
                );
            }
            ContentId = contentId;
        }
    }

    private static void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(extension))
        {
            throw new ArgumentException("File must have an extension.", nameof(fileName));
        }

        if (DangerousExtensions.Contains(extension))
        {
            throw new ArgumentException(
                $"File type {extension} is not allowed for security reasons.",
                nameof(fileName)
            );
        }

        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException(
                $"File type {extension} is not supported. Allowed types: {string.Join(", ", AllowedExtensions)}",
                nameof(fileName)
            );
        }
    }

    private static void ValidateFileSize(Stream content)
    {
        if (content.Length > MaxAttachmentSize)
        {
            throw new InvalidOperationException(
                $"Attachment size ({content.Length} bytes) exceeds the maximum allowed size of {MaxAttachmentSize} bytes."
            );
        }
    }

    public static EmailAttachment FromStream(
        Stream stream,
        string fileName,
        string? mimeType = null,
        bool isInline = false,
        string? contentId = null
    )
    {
        ArgumentNullException.ThrowIfNull(stream);
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
        ArgumentNullException.ThrowIfNull(bytes);
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

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The specified file was not found.", filePath);
        }

        try
        {
            var stream = File.OpenRead(filePath);
            var fileName = Path.GetFileName(filePath);
            var resolvedMimeType = mimeType ?? GetMimeTypeFromFileName(fileName);
            return FromStream(stream, fileName, resolvedMimeType, isInline, contentId);
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not FileNotFoundException)
        {
            throw new InvalidOperationException($"Failed to read file: {filePath}", ex);
        }
    }

    //for internal use only
    internal System.Net.Mail.Attachment ToSystemMailAttachment()
    {
        ThrowIfDisposed();

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
            ".html" or ".htm" => "text/html",
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

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(EmailAttachment));
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Content?.Dispose();
            }

            Content = null!;
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~EmailAttachment()
    {
        Dispose(false);
    }
}
