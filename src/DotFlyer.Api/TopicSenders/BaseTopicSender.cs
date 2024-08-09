namespace DotFlyer.Api.TopicSenders;

/// <summary>
/// The base topic sender.
/// </summary>
public abstract class BaseTopicSender
{
    private readonly ServiceBusSender _serviceBusSender;

    public abstract string SenderClientName { get; }

    public BaseTopicSender(IAzureClientFactory<ServiceBusSender> azureClientFactory)
    {
        _serviceBusSender = azureClientFactory.CreateClient(SenderClientName);
    }

    /// <summary>
    /// Sends a message to the topic.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Task"/>.</returns>
    public async Task SendMessageAsync<T>(T message, CancellationToken cancellationToken) =>
        await _serviceBusSender.SendMessageAsync(new(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message))), cancellationToken);
}
