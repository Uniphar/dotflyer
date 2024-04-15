namespace DotFlyer.Service;

/// <summary>
/// Azure Service bus message processor.
/// </summary>
public class AzureServiceBusMessageProcessor : IMessageProcessor, IAsyncDisposable
{
    private readonly ILogger<MessageProcessingService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ServiceBusAdministrationClient _serviceBusAdministrationClient;
    private readonly IEmailSender _emailSender;
    private readonly ServiceBusProcessor _emailTopicProcessor;

    /// <summary>
    /// Creates a new instance of the <see cref="AzureServiceBusMessageProcessor"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="configuration">The <see cref="ILogger"/> instance containing application configuration.</param>
    /// <param name="serviceBusAdministrationClient">The <see cref="ServiceBusAdministrationClient"/> instance that allows to manage namespace entities.</param>
    /// <param name="azureClientFactory">The <see cref="IAzureClientFactory"/> instance that is used to create Service Bus processors.</param>
    /// <param name="emailSender">The <see cref="IEmailSender"/> instance.</param>
    public AzureServiceBusMessageProcessor(
        ILogger<MessageProcessingService> logger,
        IConfiguration configuration,
        ServiceBusAdministrationClient serviceBusAdministrationClient,
        IAzureClientFactory<ServiceBusProcessor> azureClientFactory,
        IEmailSender emailSender)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceBusAdministrationClient = serviceBusAdministrationClient;
        _emailSender = emailSender;

        _emailTopicProcessor = azureClientFactory.CreateClient("email-topic-processor");
    }

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

        await CreateTopicAndSubscriptionIfNotExist(
            _serviceBusAdministrationClient,
            _configuration["AzureServiceBus:TopicNameForEmail"]!,
            _configuration["AzureServiceBus:SubscriptionName"]!,
            _cancellationToken);

        _emailTopicProcessor.ProcessMessageAsync += ProcessEmailMessageAsync;
        _emailTopicProcessor.ProcessErrorAsync += ProcessEmailErrorAsync;
    }

    /// <summary>
    /// Starts service bus message processors.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        await _emailTopicProcessor.StartProcessingAsync(cancellationToken != default ? cancellationToken : _cancellationToken);
    }

    /// <summary>
    /// Stops service bus message processors.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task StopProcessingAsync(CancellationToken cancellationToken = default)
    {
        await _emailTopicProcessor.StopProcessingAsync(cancellationToken != default ? cancellationToken : _cancellationToken);
    }

    /// <summary>
    /// Processes incoming email service bus message.
    /// </summary>
    /// <param name="args">The <see cref="ProcessMessageEventArgs"/> instance representing message information.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    private async Task ProcessEmailMessageAsync(ProcessMessageEventArgs args)
    {
        EmailMessage? emailMessage = JsonSerializer.Deserialize<EmailMessage>(args.Message.Body.ToString());

        if (emailMessage != null)
        {
            try
            {
                await _emailSender.SendAsync(emailMessage, _cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email message: {ex.Message}");
            }
        }
        else
        {
            _logger.LogError($"Failed to deserialize email message: {args.Message.Body}");
        }
    }

    /// <summary>
    /// Processes error that occurred during email message processing.
    /// </summary>
    /// <param name="args">The <see cref="ProcessErrorEventArgs"/> instance representing error information.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    private async Task ProcessEmailErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing email message");

        await Task.CompletedTask;
    }

    private async Task CreateTopicAndSubscriptionIfNotExist(
        ServiceBusAdministrationClient serviceBusAdministrationClient,
        string topicName,
        string subscriptionName,
        CancellationToken cancellationToken = default)
    {
        if (!await serviceBusAdministrationClient.TopicExistsAsync(topicName, cancellationToken))
        {
            await serviceBusAdministrationClient.CreateTopicAsync(new CreateTopicOptions(topicName), cancellationToken);
        }

        if (!await serviceBusAdministrationClient.SubscriptionExistsAsync(topicName, subscriptionName, cancellationToken))
        {
            await serviceBusAdministrationClient.CreateSubscriptionAsync(topicName, subscriptionName, cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _emailTopicProcessor.DisposeAsync();
    }
}
