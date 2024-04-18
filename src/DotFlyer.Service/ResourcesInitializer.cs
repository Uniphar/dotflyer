namespace DotFlyer.Service;

/// <summary>
/// Initializes resources required by the application.
/// </summary>
/// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance to log messages.</param>
/// <param name="configuration">The <see cref="IConfiguration"/> instance to access application configuration.</param>
/// <param name="topicProcessorFactory">The <see cref="ITopicProcessorFactory"/> instance to create topics and subscriptions.</param>
/// <param name="azureDataExplorerClient"></param>
public class ResourcesInitializer(
    ILogger<ResourcesInitializer> logger,
    IConfiguration configuration,
    ITopicProcessorFactory topicProcessorFactory,
    IAzureDataExplorerClient azureDataExplorerClient)
{
    /// <summary>
    /// Initializes resources required by the application.
    /// </summary>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InitializeAsync()
    {
        try
        {
            await topicProcessorFactory.CreateTopicAndSubscriptionIfNotExistAsync(
                configuration["AzureServiceBus:TopicNameForEmail"]!,
                configuration["AzureServiceBus:SubscriptionName"]!);

            await azureDataExplorerClient.CreateOrUpdateTablesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing resources.");
            throw;
        }
    }
}
