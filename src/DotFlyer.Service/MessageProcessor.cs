namespace DotFlyer.Service;

public class MessageProcessor(
    ILogger<MessageProcessor> logger,
    IConfiguration configuration,
    ServiceBusAdministrationClient serviceBusAdministrationClient,
    ServiceBusProcessor serviceBusProcessor,
    ISendGridClient sendGridClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await serviceBusAdministrationClient.TopicExistsAsync(configuration["ServiceBus:TopicName"], stoppingToken))
        {
            await serviceBusAdministrationClient.CreateTopicAsync(new CreateTopicOptions(configuration["ServiceBus:TopicName"]), stoppingToken);
        }

        if (!await serviceBusAdministrationClient.SubscriptionExistsAsync(configuration["ServiceBus:TopicName"], configuration["ServiceBus:SubscriptionName"], stoppingToken))
        {
            await serviceBusAdministrationClient.CreateSubscriptionAsync(configuration["ServiceBus:TopicName"], configuration["ServiceBus:SubscriptionName"], stoppingToken);
        }

        serviceBusProcessor.ProcessMessageAsync += async (args) =>
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
                
                var result = await sendGridClient.SendEmailAsync(sendGridMessage, stoppingToken);

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

            await Task.CompletedTask;
        };

        serviceBusProcessor.ProcessErrorAsync += async (args) =>
        {
            logger.LogError(args.Exception, "Error processing message");

            await Task.CompletedTask;
        };

        await serviceBusProcessor.StartProcessingAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }

        await serviceBusProcessor.StopProcessingAsync(stoppingToken);
    }
}
