namespace DotFlyer.Service.TopicProcessors;

/// <summary>
/// Base class for a topic processor.
/// </summary>
/// <param name="logger">The <see cref="ILogger"/> instance to log messages.</param>
public class BaseTopicProcessor(ILogger logger)
{
    /// <summary>
    /// Processes incoming service bus message.
    /// </summary>
    /// <param name="args">The <see cref="ProcessMessageEventArgs"/> instance representing message information.</param>
    /// <param name="sender">The <see cref="ISender"/> instance to send the message.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task ProcessMessageAsync<TMessage>(ProcessMessageEventArgs args, ISender<TMessage> sender)
    {
        try
        {
            TMessage? message = JsonSerializer.Deserialize<TMessage>(args.Message.Body.ToString())
                ?? throw new ArgumentException("The message data is deserialized to null");

            await sender.SendAsync(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to send: {args.Message.Body}");
        }
    }

    /// <summary>
    /// Processes error that occurred during message processing.
    /// </summary>
    /// <param name="args">The <see cref="ProcessErrorEventArgs"/> instance representing error information.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error processing email message");

        await Task.CompletedTask;
    }
}
