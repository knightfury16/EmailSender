namespace EmailSenderLib.Models;

public class EmailSendResponse
{
    private static readonly string SuccessMessage = "Email sent successfully.";
    private static readonly string FailureMessage = "Email sending failed.";

    /// <summary>
    /// Gets a value indicating whether the email was sent successfully.
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Gets or sets the unique identifier of the sent message, if available.
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the email was sent.
    /// </summary>
    public DateTimeOffset SentAt { get; set; }

    /// <summary>
    /// Gets the error message if the email sending failed.
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Gets or sets additional metadata associated with the email sending operation.
    /// </summary>
    public Dictionary<string, object> MetaData { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Initializes a new instance of the EmailSendResponse class.
    /// </summary>
    /// <param name="isSuccess">Whether the email was sent successfully.</param>
    /// <param name="errorMessage">The error message if the operation failed.</param>
    /// <param name="messageId">The unique identifier of the sent message.</param>
    /// <param name="sentAt">The timestamp when the email was sent.</param>
    private EmailSendResponse(
        bool isSuccess,
        string? errorMessage,
        string? messageId,
        DateTimeOffset sentAt
    )
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        SentAt = sentAt;
        MessageId = messageId;
    }

    /// <summary>
    /// Creates a successful email sending response.
    /// </summary>
    /// <param name="messageId">Optional message identifier.</param>
    /// <returns>A new EmailSendResponse instance indicating success.</returns>
    public static EmailSendResponse Success(string? messageId = null)
    {
        return new EmailSendResponse(true, SuccessMessage, messageId, DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Creates a failed email sending response.
    /// </summary>
    /// <param name="errorMessage">Optional error message. If not provided, a default message will be used.</param>
    /// <param name="messageId">Optional message identifier.</param>
    /// <returns>A new EmailSendResponse instance indicating failure.</returns>
    public static EmailSendResponse Failure(string? errorMessage = null, string? messageId = null)
    {
        return new EmailSendResponse(
            false,
            errorMessage ?? FailureMessage,
            messageId,
            DateTimeOffset.UtcNow
        );
    }

    /// <summary>
    /// Gets the default success message.
    /// </summary>
    /// <returns>The default success message string.</returns>
    public static string GetEmailResponseSuccessMessage()
    {
        return SuccessMessage;
    }

    /// <summary>
    /// Gets the default failure message.
    /// </summary>
    /// <returns>The default failure message string.</returns>
    public static string GetEmailResponseFailureMessage()
    {
        return FailureMessage;
    }
}
