namespace DotFlyer.Service.TopicProcessors;

/// <summary>
/// Azure Service bus topic processor factory.
/// </summary>
public interface ITopicProcessorFactory
{
    public ServiceBusProcessor CreateTopicProcessor(string processorName);

    public Task CreateTopicAndSubscriptionIfNotExistAsync(string topicName, string subscriptionName, CancellationToken cancellationToken = default);
}
