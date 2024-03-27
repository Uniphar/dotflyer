namespace DotFlyer.Service;

/// <summary>
/// Azure Service bus message processor.
/// </summary>
/// <param name="logger">The <see cref="ILogger"/> instance.</param>
/// <param name="configuration">The <see cref="ILogger"/> instance containing application configuration.</param>
/// <param name="serviceBusAdministrationClient">The <see cref="ServiceBusAdministrationClient"/> instance that allows to manage namespace entities.</param>
/// <param name="serviceBusProcessor">The <see cref="ServiceBusProcessor"/> instance that processes incoming messages.</param>
/// <param name="sendGridClient">The <see cref="ISendGridClient"/> instance that sends emails.</param>
public class AzureServiceBusMessageProcessor(
        ILogger<MessageProcessingService> logger,
        IConfiguration configuration,
        ServiceBusAdministrationClient serviceBusAdministrationClient,
        ServiceBusProcessor serviceBusProcessor,
        ISendGridClient sendGridClient) : IMessageProcessor
{
    /// <summary>
    /// The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.
    /// </summary>
    private CancellationToken _cancellationToken;

    /// <summary>
    /// Initializes service bus message processor.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _cancellationToken = cancellationToken;

        if (!await serviceBusAdministrationClient.TopicExistsAsync(configuration["ServiceBus:TopicName"], _cancellationToken))
        {
            await serviceBusAdministrationClient.CreateTopicAsync(new CreateTopicOptions(configuration["ServiceBus:TopicName"]), _cancellationToken);
        }

        if (!await serviceBusAdministrationClient.SubscriptionExistsAsync(configuration["ServiceBus:TopicName"], configuration["ServiceBus:SubscriptionName"], _cancellationToken))
        {
            await serviceBusAdministrationClient.CreateSubscriptionAsync(configuration["ServiceBus:TopicName"], configuration["ServiceBus:SubscriptionName"], _cancellationToken);
        }

        serviceBusProcessor.ProcessMessageAsync += ProcessMessageAsync;

        serviceBusProcessor.ProcessErrorAsync += ProcessErrorAsync;
    }

    /// <summary>
    /// Starts service bus message processor.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task StartProcessingAsync(CancellationToken cancellationToken = default) =>
        await serviceBusProcessor.StartProcessingAsync(cancellationToken != default ? cancellationToken : _cancellationToken);

    /// <summary>
    /// Stops service bus message processor.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task StopProcessingAsync(CancellationToken cancellationToken = default) =>
        await serviceBusProcessor.StopProcessingAsync(cancellationToken != default ? cancellationToken : _cancellationToken);

    /// <summary>
    /// Processes incoming service bus message.
    /// </summary>
    /// <param name="args">The <see cref="ProcessMessageEventArgs"/> instance representing message information.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        EmailMessage? emailMessage = JsonSerializer.Deserialize<EmailMessage>(args.Message.Body.ToString());

        if (emailMessage != null)
        {
            SendGridMessage sendGridMessage = new()
            {
                From = new(emailMessage.FromEmail, emailMessage.FromName),
                Subject = emailMessage.Subject,
                PlainTextContent = emailMessage.Body
            };

            foreach (var emailRecipient in emailMessage.To)
            {
                sendGridMessage.AddTo(new EmailAddress(emailRecipient.Email, emailRecipient.Name));
            }

            var result = await sendGridClient.SendEmailAsync(sendGridMessage, _cancellationToken);

            if (result.StatusCode != HttpStatusCode.Accepted)
            {
                var errorMessage = await result.Body.ReadAsStringAsync();

                logger.LogError($"Failed to send email message: {errorMessage}");
            }
        }
        else
        {
            logger.LogError($"Failed to deserialize email message: {args.Message.Body}");
        }
    }

    /// <summary>
    /// Processes error that occurred during message processing.
    /// </summary>
    /// <param name="args">The <see cref="ProcessErrorEventArgs"/> instance representing error information.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    private async Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error processing message");

        await Task.CompletedTask;
    }
}
