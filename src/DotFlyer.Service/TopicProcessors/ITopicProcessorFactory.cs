namespace DotFlyer.Service.TopicProcessors;

/// <summary>
/// Azure Service bus topic processor factory.
/// </summary>
public interface ITopicProcessorFactory
{
    /// <summary>
    /// Creates a topic processor.
    /// </summary>
    /// <param name="processorName">The topic processor name.</param>
    /// <returns>The <see cref="ServiceBusProcessor"/> instance.</returns>
    public ServiceBusProcessor CreateTopicProcessor(string processorName);
}
