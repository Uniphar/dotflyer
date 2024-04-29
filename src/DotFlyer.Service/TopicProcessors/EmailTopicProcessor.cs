namespace DotFlyer.Service.TopicProcessors;

/// <summary>
/// Email topic processor.
/// </summary>
/// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance to log messages.</param>
/// <param name="emailSender">The <see cref="IEmailSender"/> instance to send email messages.</param>
public class EmailTopicProcessor(
    ILogger<EmailTopicProcessor> logger,
    IEmailSender emailSender) : BaseTopicProcessor(logger), ITopicProcessor
{
    public const string ProcessorName = "email-topic-processor";

    public string Name => ProcessorName;

    /// <summary>
    /// Processes incoming email service bus message.
    /// </summary>
    /// <param name="args">The <see cref="ProcessMessageEventArgs"/> instance representing message information.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task ProcessMessageAsync(ProcessMessageEventArgs args) => await ProcessMessageAsync(args, emailSender);
}
