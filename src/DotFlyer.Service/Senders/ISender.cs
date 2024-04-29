namespace DotFlyer.Service.Senders;

/// <summary>
/// Represents a sender.
/// </summary>
public interface ISender<TMessage>
{
    /// <summary>
    /// Sends a message.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task SendAsync(TMessage message, CancellationToken cancellationToken = default);
}
