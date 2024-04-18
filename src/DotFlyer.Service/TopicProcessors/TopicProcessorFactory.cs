namespace DotFlyer.Service.TopicProcessors;

public class TopicProcessorFactory(
    IServiceProvider serviceProvider,
    IAzureClientFactory<ServiceBusProcessor> azureClientFactory) : ITopicProcessorFactory
{
    public ServiceBusProcessor CreateTopicProcessor(string topicProcessorName)
    {
        var topicProcessor = topicProcessorName switch
        {
            EmailTopicProcessor.ProcessorName => serviceProvider.GetRequiredService<EmailTopicProcessor>(),
            _ => throw new ArgumentException($"Unknown topic processor name: {topicProcessorName}")
        };
        
        ServiceBusProcessor serviceBusProcessor = azureClientFactory.CreateClient(topicProcessor.Name);

        serviceBusProcessor.ProcessMessageAsync += topicProcessor.ProcessMessageAsync;
        serviceBusProcessor.ProcessErrorAsync += topicProcessor.ProcessErrorAsync;

        return serviceBusProcessor;
    }
}
