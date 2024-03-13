namespace DotFlyer.Service;

public class MessageProcessor : BackgroundService
{
    private readonly ILogger<MessageProcessor> _logger;

    public MessageProcessor(ILogger<MessageProcessor> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        _logger.LogInformation("Worker stopped at: {time}", DateTimeOffset.Now);
    }
}
