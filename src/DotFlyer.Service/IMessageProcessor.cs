namespace DotFlyer.Service;

public interface IMessageProcessor
{
    /// <summary>
    /// Initializes the message processor.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts the message processor. The processor must be initialized if required before calling this method.
    /// The message processor will call this method and then wait until cancellation is requested.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task StartProcessingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the message processor.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task StopProcessingAsync(CancellationToken cancellationToken = default);
}
