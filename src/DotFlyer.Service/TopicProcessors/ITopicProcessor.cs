namespace DotFlyer.Service.TopicProcessors;

public interface ITopicProcessor
{
    public string Name { get; }

    public Task ProcessMessageAsync(ProcessMessageEventArgs args);

    public Task ProcessErrorAsync(ProcessErrorEventArgs args);
}
