namespace DotFlyer.Service.TopicProcessors;

public class TopicProcessorFactory(
    IServiceProvider serviceProvider,
    IAzureClientFactory<ServiceBusProcessor> azureClientFactory,
    ServiceBusAdministrationClient serviceBusAdministrationClient) : ITopicProcessorFactory
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

    public async Task CreateTopicAndSubscriptionIfNotExistAsync(
        string topicName,
        string subscriptionName,
        CancellationToken cancellationToken = default)
    {
        if (!await serviceBusAdministrationClient.TopicExistsAsync(topicName, cancellationToken))
        {
            await serviceBusAdministrationClient.CreateTopicAsync(new CreateTopicOptions(topicName), cancellationToken);
        }

        if (!await serviceBusAdministrationClient.SubscriptionExistsAsync(topicName, subscriptionName, cancellationToken))
        {
            await serviceBusAdministrationClient.CreateSubscriptionAsync(topicName, subscriptionName, cancellationToken);
        }
    }
}
