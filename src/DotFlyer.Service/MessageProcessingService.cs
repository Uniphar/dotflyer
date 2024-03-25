namespace DotFlyer.Service;

/// <summary>
/// Message processing service.
/// </summary>
/// <param name="messageProcessor">The <see cref="IMessageProcessor"/> instance that processes incoming messages.</param>
public class MessageProcessingService(
    IMessageProcessor messageProcessor) : BackgroundService
{
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
