namespace DotFlyer.Service.TopicProcessors;

/// <summary>
/// Interface for a topic processor.
/// </summary>
public interface ITopicProcessor
{
    /// <summary>
    /// Name of the topic processor.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Method to process a message.
    /// </summary>
    /// <param name="args">The <see cref="ProcessMessageEventArgs"/> instance representing message information.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task ProcessMessageAsync(ProcessMessageEventArgs args);

    /// <summary>
    /// Method to process an error.
    /// </summary>
    /// <param name="args">The <see cref="ProcessMessageEventArgs"/> instance representing message information.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task ProcessErrorAsync(ProcessErrorEventArgs args);
}
