namespace DotFlyer.Service.TopicProcessors;

/// <summary>
/// SMS topic processor.
/// </summary>
/// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance to log messages.</param>
/// <param name="smsSender">The <see cref="ISMSSender"/> instance to send SMS messages.</param>
public class SMSTopicProcessor(
    ILogger<SMSTopicProcessor> logger,
    ISMSSender smsSender) : BaseTopicProcessor(logger), ITopicProcessor
{
    public const string ProcessorName = "sms-topic-processor";

    public string Name => ProcessorName;

    /// <summary>
    /// Processes incoming SMS service bus message.
    /// </summary>
    /// <param name="args">The <see cref="ProcessMessageEventArgs"/> instance representing message information.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task ProcessMessageAsync(ProcessMessageEventArgs args) => await ProcessMessageAsync(args, smsSender);
}
