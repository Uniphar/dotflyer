namespace DotFlyer.Service;

/// <summary>
/// Represents a message processor.
/// </summary>
public interface IMessagesProcessor
{
    /// <summary>
    /// Starts the message processor. The processor must be initialized if required before calling this method.
    /// The message processor will call this method and then wait until cancellation is requested.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task StartProcessingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the message processor after cancellation is requested.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task StopProcessingAsync(CancellationToken cancellationToken = default);
}
