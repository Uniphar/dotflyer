namespace DotFlyer.Service;

/// <summary>
/// Azure Service bus messages processor.
/// </summary>
public class AzureServiceBusMessagesProcessor : IMessagesProcessor, IAsyncDisposable
{
    private readonly ITopicProcessorFactory _topicProcessorFactory;
    private readonly ServiceBusProcessor _emailTopicProcessor;
    private readonly ServiceBusProcessor _smsTopicProcessor;

    /// <summary>
    /// Creates a new instance of the <see cref="AzureServiceBusMessagesProcessor"/> class.
    /// </summary>
    /// <param name="topicProcessorFactory">The <see cref="ITopicProcessorFactory"/> instance that is used to create Service Bus processors.</param
    public AzureServiceBusMessagesProcessor(ITopicProcessorFactory topicProcessorFactory)
    {
        _topicProcessorFactory = topicProcessorFactory;

        _emailTopicProcessor = _topicProcessorFactory.CreateTopicProcessor(EmailTopicProcessor.ProcessorName);
        _smsTopicProcessor = _topicProcessorFactory.CreateTopicProcessor(SMSTopicProcessor.ProcessorName);
    }

    /// <summary>
    /// Starts service bus messages processors.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        await _emailTopicProcessor.StartProcessingAsync(cancellationToken);
        await _smsTopicProcessor.StartProcessingAsync(cancellationToken);
    }

    /// <summary>
    /// Stops service bus messages processors.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task StopProcessingAsync(CancellationToken cancellationToken = default)
    {
        await _emailTopicProcessor.StopProcessingAsync(cancellationToken);
        await _smsTopicProcessor.StopProcessingAsync(cancellationToken);
    }

    /// <summary>
    /// Disposes the service bus messages processor.
    /// </summary>
    /// <returns>The <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await _emailTopicProcessor.DisposeAsync();
        await _smsTopicProcessor.DisposeAsync();
    }
}
