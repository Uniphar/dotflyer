namespace DotFlyer.Service.TopicProcessors;

/// <summary>
/// SMS topic processor.
/// </summary>
/// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance to log messages.</param>
/// <param name="smsSender">The <see cref="ISMSSender"/> instance to send SMS messages.</param>
public class SMSTopicProcessor(
    ILogger<SMSTopicProcessor> logger,
    ISMSSender smsSender) : ITopicProcessor
{
    public const string ProcessorName = "sms-topic-processor";

    public string Name => ProcessorName;

    /// <summary>
    /// Processes incoming SMS service bus message.
    /// </summary>
    /// <param name="args">The <see cref="ProcessMessageEventArgs"/> instance representing message information.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            SMSMessage? smsMessage = JsonSerializer.Deserialize<SMSMessage>(args.Message.Body.ToString())
                ?? throw new ArgumentException("The sms message data is deserialized to null");

            await smsSender.SendAsync(smsMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to send an SMS: {args.Message.Body}");
        }
    }

    /// <summary>
    /// Processes error that occurred during SMS message processing.
    /// </summary>
    /// <param name="args">The <see cref="ProcessErrorEventArgs"/> instance representing error information.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error processing SMS message");

        await Task.CompletedTask;
    }
}
