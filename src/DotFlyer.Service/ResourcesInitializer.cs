﻿namespace DotFlyer.Service;

/// <summary>
/// Initializes resources required by the application.
/// </summary>
/// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance to log messages.</param>
/// <param name="configuration">The <see cref="IConfiguration"/> instance to access application configuration.</param>
/// <param name="serviceBusAdministrationClient">The <see cref="ServiceBusAdministrationClient"/> instance to interact with Azure Service Bus.</param>
/// <param name="azureDataExplorerClient">The <see cref="IAzureDataExplorerClient"/> instance to interact with Azure Data Explorer.</param>
/// <param name="blobServiceClient">The <see cref="BlobServiceClient"/> instance to interact with Azure Blob Storage.</param>
public class ResourcesInitializer(
    ILogger<ResourcesInitializer> logger,
    IConfiguration configuration,
    ServiceBusAdministrationClient serviceBusAdministrationClient,
    IAzureDataExplorerClient azureDataExplorerClient,
    BlobServiceClient blobServiceClient)
{
    /// <summary>
    /// Initializes resources required by the application.
    /// </summary>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InitializeAsync()
    {
        try
        {
            var duplicateDetectionTimeWindow = TimeSpan.FromSeconds(int.TryParse(configuration["AzureServiceBus:DuplicateDetectionTimeWindowInSeconds"], out int result) ? result : 600);

            await CreateTopicAndSubscriptionIfNotExistAsync(configuration["AzureServiceBus:TopicNameForEmail"]!, duplicateDetectionTimeWindow, configuration["AzureServiceBus:SubscriptionName"]!);
            await CreateTopicAndSubscriptionIfNotExistAsync(configuration["AzureServiceBus:TopicNameForSMS"]!, duplicateDetectionTimeWindow, configuration["AzureServiceBus:SubscriptionName"]!);

            await azureDataExplorerClient.CreateOrUpdateTablesAsync();

            await blobServiceClient.GetBlobContainerClient(configuration["AzureStorage:EmailAttachmentsContainerName"]!).CreateIfNotExistsAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing resources.");
            throw;
        }
    }

    /// <summary>
    /// Creates a topic and subscription if they do not exist.
    /// </summary>
    /// <param name="topicName">The name of the topic.</param>
    /// <param name="duplicateDetectionTimeWindow">The time window for duplicate detection.</param>
    /// <param name="subscriptionName">The name of the subscription.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
    /// <returns></returns>
    private async Task CreateTopicAndSubscriptionIfNotExistAsync(string topicName, TimeSpan duplicateDetectionTimeWindow, string subscriptionName)
    {
        if (await serviceBusAdministrationClient.TopicExistsAsync(topicName))
        {
            var topic = await serviceBusAdministrationClient.GetTopicAsync(topicName);

            if (topic.Value.DuplicateDetectionHistoryTimeWindow != duplicateDetectionTimeWindow)
            {
                topic.Value.DuplicateDetectionHistoryTimeWindow = duplicateDetectionTimeWindow;

                await serviceBusAdministrationClient.UpdateTopicAsync(topic.Value);
            }
        }
        else
        {
            await serviceBusAdministrationClient.CreateTopicAsync(new CreateTopicOptions(topicName)
            {
                RequiresDuplicateDetection = true,
                DuplicateDetectionHistoryTimeWindow = duplicateDetectionTimeWindow
            });
        }

        if (!await serviceBusAdministrationClient.SubscriptionExistsAsync(topicName, subscriptionName))
        {
            await serviceBusAdministrationClient.CreateSubscriptionAsync(topicName, subscriptionName);
        }
    }
}
