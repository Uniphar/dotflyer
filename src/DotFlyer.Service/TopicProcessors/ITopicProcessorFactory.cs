namespace DotFlyer.Service.TopicProcessors;

/// <summary>
/// Azure Service bus topic processor factory.
/// </summary>
public interface ITopicProcessorFactory
{
    public ServiceBusProcessor CreateTopicProcessor(string processorName);
}
