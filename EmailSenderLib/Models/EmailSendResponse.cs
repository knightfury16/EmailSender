namespace EmailSenderLib.Models;

public class EmailSendResponse
{
    private static readonly string SuccessMessage = "Email sent successfully.";
    private static readonly string FailureMessage = "Email sending failed.";

    /// <summary>
    /// Gets a value indicating whether the email was sent successfully.
    /// </summary>
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the unique identifier of the sent message, if available.
    /// </summary>
    public string? MessageId { get; }

    /// <summary>
    /// Gets or sets the timestamp when the email was sent.
    /// </summary>
    public DateTimeOffset SentAt { get; }

    /// <summary>
    /// Gets the error message if the email sending failed.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets or sets additional metadata associated with the email sending operation.
    /// </summary>
    public IReadOnlyDictionary<string, object> MetaData { get; }

    /// <summary>
    /// Initializes a new instance of the EmailSendResponse class.
    /// </summary>
    private EmailSendResponse(
        bool isSuccess,
        string? errorMessage = null,
        string? messageId = null,
        IReadOnlyDictionary<string, object>? metaData = null,
        DateTimeOffset? sentAt = null
    )
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        MessageId = messageId;
        MetaData = metaData ?? new Dictionary<string, object>();
        SentAt = sentAt ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Creates a successful email sending response.
    /// </summary>
    /// <param name="messageId">Optional message identifier.</param>
    /// <returns>A new EmailSendResponse instance indicating success.</returns>
    public static EmailSendResponse Success(
        string? messageId = null,
        IReadOnlyDictionary<string, object>? metaData = null,
        DateTimeOffset? sentAt = null
    )
    {
        return new EmailSendResponse(true, null, messageId, metaData, sentAt);
    }

    /// <summary>
    /// Creates a failed email sending response.
    /// </summary>
    /// <param name="errorMessage">Optional error message. If not provided, a default message will be used.</param>
    /// <param name="messageId">Optional message identifier.</param>
    /// <returns>A new EmailSendResponse instance indicating failure.</returns>
    public static EmailSendResponse Failure(
        string? errorMessage = null,
        string? messageId = null,
        IReadOnlyDictionary<string, object>? metaData = null,
        DateTimeOffset? sentAt = null
    )
    {
        return new EmailSendResponse(
            false,
            errorMessage ?? FailureMessage,
            messageId,
            metaData,
            sentAt
        );
    }

    /// <summary>
    /// Gets the default success message.
    /// </summary>
    /// <returns>The default success message string.</returns>
    private static string GetEmailResponseSuccessMessage()
    {
        return SuccessMessage;
    }

    /// <summary>
    /// Gets the default failure message.
    /// </summary>
    /// <returns>The default failure message string.</returns>
    private static string GetEmailResponseFailureMessage()
    {
        return FailureMessage;
    }
}
