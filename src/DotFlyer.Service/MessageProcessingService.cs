namespace DotFlyer.Service;

/// <summary>
/// Message processing service.
/// </summary>
/// <param name="messageProcessor">The <see cref="IMessageProcessor"/> instance that processes incoming messages.</param>
public class MessageProcessingService(IMessageProcessor messageProcessor) : BackgroundService
{
    /// <summary>
    /// Executes the message processing service.
    /// </summary>
    /// <param name="stoppingToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageProcessor.InitializeAsync(stoppingToken);

        await messageProcessor.StartProcessingAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }

        await messageProcessor.StopProcessingAsync(stoppingToken);
    }
}
