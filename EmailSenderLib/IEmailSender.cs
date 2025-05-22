namespace EmailSenderLib;

using EmailSenderLib.Models;



public interface IEmailSender
{
    /// <summary>
    /// Sends a single email asynchronously.
    /// </summary>
    /// <param name="request">The email request containing all necessary information to send the email.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the email sending response.</returns>
    public Task<EmailSendResponse> SendEmailAsync(
        EmailSendRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends multiple emails asynchronously in bulk.
    /// </summary>
    /// <param name="requests">Collection of email requests to be sent.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the email sending response.</returns>
    public Task<IEnumerable<EmailSendResponse>> SendBulkEmailAsync(
        ICollection<EmailSendRequest> requests,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a single templated email asynchronously.
    /// </summary>
    /// <param name="request">The templated email request containing template ID and content.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the email sending response.</returns>
    public Task<EmailSendResponse> SendTemplatedEmailAsync(
        TemplatedEmailRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends multiple templated emails asynchronously in bulk.
    /// </summary>
    /// <param name="requests">Collection of templated email requests to be sent.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the email sending response.</returns>
    public Task<IEnumerable<EmailSendResponse>> SendBulkTemplatedEmailAsync(
        ICollection<TemplatedEmailRequest> requests,
        CancellationToken cancellationToken = default
    );
}

