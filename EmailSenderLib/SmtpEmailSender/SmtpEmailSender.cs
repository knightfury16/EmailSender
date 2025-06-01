using System.Net;
using System.Net.Mail;
using EmailSenderLib.Models;
using EmailSenderLib.TemplateRenderer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmailSenderLib.SmtpEmailSender;

public sealed class SmtpEmailSender : IEmailSender, IDisposable
{
    private readonly SmtpEmailSettings _settings;
    private readonly ILogger<SmtpEmailSettings> _logger;
    private readonly ITemplateRenderer? _templateRenderer;
    private readonly SmtpClient _smtpClient;
    private bool _disposed;

    public SmtpEmailSender(
        IOptions<SmtpEmailSettings> settings,
        ILogger<SmtpEmailSettings> logger,
        ITemplateRenderer? templateRenderer = null
    )
    {
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger;
        _templateRenderer = templateRenderer;
        _smtpClient = CreateSmtpClient();
    }

    public async Task<EmailSendResponse> SendEmailAsync(
        EmailRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger?.LogDebug("Preparing to send email with subject {Subject} and messageId {MessageId}", 
                request.Subject, request.MessageId);

            using var message = CreateMailMessage(request);

            _logger?.LogInformation(
                "Sending email to {Recipients} with subject: {Subject} and messageId: {MessageId}",
                string.Join(",", request.To.Select(t => t.Address)),
                message.Subject,
                request.MessageId
            );

            await _smtpClient.SendMailAsync(message, cancellationToken);

            var messageId = message.Headers[SendRequest.MessageIdKey];

            _logger?.LogInformation("Email sent successfully with messageId {MessageId}", messageId);

            return EmailSendResponse.Success();
        }
        catch (OperationCanceledException)
        {
            _logger?.LogWarning(
                "Email sending was cancelled. Subject: {Subject}, MessageId: {MessageId}",
                request.Subject,
                request.MessageId
            );
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(
                ex,
                "Failed to send email. Subject: {Subject}, MessageId: {MessageId}, Error: {ErrorMessage}",
                request.Subject,
                request.MessageId,
                ex.Message
            );
            return EmailSendResponse.Failure($"Failed to send email: {ex.Message}");
        }
    }

    public async Task<EmailSendResponse> SendTemplatedEmailAsync(
        TemplatedEmailRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        if (_templateRenderer == null)
        {
            _logger?.LogError(
                "Template renderer is not configured. Unable to send templated email."
            );
            return EmailSendResponse.Failure("Template Renderer is not configured.");
        }
        try
        {
            var renderedTemplate = await _templateRenderer.RenderTemplateAsync(
                request.TemplateContent,
                request.TemplateId,
                cancellationToken
            );

            var emailRequest = new EmailRequest(request.To)
            {
                From = request.From,
                Subject = request.Subject,
                HtmlContent = renderedTemplate.HtmlContent,
                TextContent = renderedTemplate.TextContent,
                Priority = request.Priority,
            };

            emailRequest.AddBcc(request.Bcc.ToArray());
            emailRequest.AddCc(request.Cc.ToArray());
            emailRequest.AddHeaders(request.Headers);

            return await SendEmailAsync(emailRequest, cancellationToken);
        }
        catch (System.Exception ex)
        {
            return EmailSendResponse.Failure(ex.Message);
        }
    }

    public async Task<IEnumerable<EmailSendResponse>> SendBulkEmailAsync(
        ICollection<EmailRequest> requests,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(requests);

        var tasks = requests.Select(request => 
            Task.Run(() => SendEmailAsync(request, cancellationToken), cancellationToken)
        );

        return await Task.WhenAll(tasks);
    }

    public async Task<IEnumerable<EmailSendResponse>> SendBulkTemplatedEmailAsync(
        ICollection<TemplatedEmailRequest> requests,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(requests);

        var tasks = requests.Select(request => 
            Task.Run(() => SendTemplatedEmailAsync(request, cancellationToken), cancellationToken)
        );

        return await Task.WhenAll(tasks);
    }

    private SmtpClient CreateSmtpClient()
    {
        if (string.IsNullOrEmpty(_settings.SmtpServer))
        {
            throw new InvalidOperationException("Smtp server is required.");
        }

        var smtpServer = _settings.SmtpServer;
        var port = _settings.SmtpPort;
        var timeOut = _settings.TimeoutMilliseconds;
        var enableSsl = _settings.EnableSsl;

        var smtpClient = new SmtpClient
        {
            Host = smtpServer,
            Port = port,
            EnableSsl = enableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = timeOut,
            UseDefaultCredentials = false,
        };

        smtpClient.Credentials = CreateNetworkCredential();

        return smtpClient;
    }

    private NetworkCredential CreateNetworkCredential()
    {
        if (string.IsNullOrEmpty(_settings.UserName) || string.IsNullOrEmpty(_settings.Password))
        {
            throw new ArgumentNullException(
                "Username and password is required to create credential",
                "Credentials"
            );
        }

        var userName = _settings.UserName;
        var password = _settings.Password;
        var domain = _settings.Domain;

        return string.IsNullOrEmpty(domain)
            ? new NetworkCredential(userName, password)
            : new NetworkCredential(userName, password, domain);
    }

    private MailMessage CreateMailMessage(EmailRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var message = new MailMessage();

        // Set the from address
        var from = GetFrom(request.From);
        message.From = from;

        // Set the to addresses
        var to = request.To.Select(t => t.ToSystemMail()).ToList();
        foreach (var t in to)
        {
            message.To.Add(t);
        }

        // Set the cc addresses
        var cc = request.Cc.Select(c => c.ToSystemMail()).ToList();
        foreach (var c in cc)
        {
            message.CC.Add(c);
        }

        // Set the bcc addresses
        var bcc = request.Bcc.Select(b => b.ToSystemMail()).ToList();
        foreach (var b in bcc)
        {
            message.Bcc.Add(b);
        }

        // Set the subject
        message.Subject = request.Subject;

        // Set the body
        // throw error if no body is provided
        if (string.IsNullOrEmpty(request.HtmlContent) && string.IsNullOrEmpty(request.TextContent))
        {
            throw new ArgumentException("Either html content or text content must be provided.");
        }
        message.Body = request.HtmlContent ?? request.TextContent;
        if (request.HtmlContent != null)
        {
            message.IsBodyHtml = true;
        }

        // Set the attachments
        foreach (var attachment in request.EmailAttachments)
        {
            message.Attachments.Add(attachment.ToSystemMailAttachment());
        }

        // Set the priority
        message.Priority = ConvertPriority(request.Priority);

        // Set the headers
        foreach (var header in request.Headers)
        {
            message.Headers.Add(header.Key, header.Value);
        }

        // Set the date
        message.Headers.Add("Date", DateTime.UtcNow.ToString("r"));

        return message;
    }

    private MailPriority ConvertPriority(EmailPriority priority)
    {
        return priority switch
        {
            EmailPriority.Low => MailPriority.Low,
            EmailPriority.Normal => MailPriority.Normal,
            EmailPriority.High => MailPriority.High,
            _ => MailPriority.Normal,
        };
    }

    private System.Net.Mail.MailAddress GetFrom(EmailAddress? from)
    {
        if (from != null)
        {
            return from.ToSystemMail();
        }
        return GetSenderFromSettings();
    }

    private System.Net.Mail.MailAddress GetSenderFromSettings()
    {
        if (string.IsNullOrEmpty(_settings.SenderEmail))
        {
            throw new InvalidOperationException("Sender email is not configured.");
        }
        return string.IsNullOrEmpty(_settings.SenderName)
            ? new System.Net.Mail.MailAddress(_settings.SenderEmail!)
            : new System.Net.Mail.MailAddress(_settings.SenderEmail!, _settings.SenderName!);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _smtpClient?.Dispose();
            _disposed = true;
        }
    }
}
