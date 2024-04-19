namespace DotFlyer.Service;

/// <summary>
/// Represents an email sender.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email message.
    /// </summary>
    /// <param name="emailMessage">The <see cref="EmailMessage"/> instance to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
}
