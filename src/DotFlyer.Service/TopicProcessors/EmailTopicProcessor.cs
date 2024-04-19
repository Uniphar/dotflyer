namespace DotFlyer.Service.TopicProcessors;

/// <summary>
/// Email topic processor.
/// </summary>
/// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance to log messages.</param>
/// <param name="emailSender">The <see cref="IEmailSender"/> instance to send email messages.</param>
public class EmailTopicProcessor(
    ILogger<EmailTopicProcessor> logger,
    IEmailSender emailSender) : ITopicProcessor
{
    public const string ProcessorName = "email-topic-processor";

    public string Name => ProcessorName;

    /// <summary>
    /// Processes incoming email service bus message.
    /// </summary>
    /// <param name="args">The <see cref="ProcessMessageEventArgs"/> instance representing message information.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            EmailMessage? emailMessage = JsonSerializer.Deserialize<EmailMessage>(args.Message.Body.ToString()) 
                ?? throw new ArgumentException("The email message data is deserialized to null");
            
            await emailSender.SendAsync(emailMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to send an email: {args.Message.Body}");
        }
    }

    /// <summary>
    /// Processes error that occurred during email message processing.
    /// </summary>
    /// <param name="args">The <see cref="ProcessErrorEventArgs"/> instance representing error information.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error processing email message");

        await Task.CompletedTask;
    }
}
