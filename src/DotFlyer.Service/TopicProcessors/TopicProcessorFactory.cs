namespace DotFlyer.Service.TopicProcessors;

/// <summary>
/// Azure Service bus topic processor factory.
/// </summary>
/// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance.</param>
/// <param name="azureClientFactory">The <see cref="IAzureClientFactory{TClient}"/> instance.</param>
public class TopicProcessorFactory(
    IServiceProvider serviceProvider,
    IAzureClientFactory<ServiceBusProcessor> azureClientFactory) : ITopicProcessorFactory
{
    /// <summary>
    /// Creates a topic processor.
    /// </summary>
    /// <param name="topicProcessorName">The topic processor name.</param>
    /// <returns>The <see cref="ServiceBusProcessor"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when an unknown topic processor name is provided.</exception>
    public ServiceBusProcessor CreateTopicProcessor(string topicProcessorName)
    {
        ITopicProcessor topicProcessor = topicProcessorName switch
        {
            EmailTopicProcessor.ProcessorName => serviceProvider.GetRequiredService<EmailTopicProcessor>(),
            SMSTopicProcessor.ProcessorName => serviceProvider.GetRequiredService<SMSTopicProcessor>(),
            _ => throw new ArgumentException($"Unknown topic processor name: {topicProcessorName}")
        };
        
        ServiceBusProcessor serviceBusProcessor = azureClientFactory.CreateClient(topicProcessor.Name);

        serviceBusProcessor.ProcessMessageAsync += topicProcessor.ProcessMessageAsync;
        serviceBusProcessor.ProcessErrorAsync += topicProcessor.ProcessErrorAsync;

        return serviceBusProcessor;
    }
}
