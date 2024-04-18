namespace DotFlyer.Service.Extensions;

/// <summary>
/// Extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a message processor and a hosted service to the service collection.
    /// </summary>
    /// <typeparam name="TMessageProcessor">Concrete type of the message processor.</typeparam>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddDotFlyerMessageProcessor<TMessageProcessor>(this IServiceCollection serviceCollection)
        where TMessageProcessor : class, IMessagesProcessor
    {
        serviceCollection.AddSingleton<IMessagesProcessor, TMessageProcessor>();
        serviceCollection.AddHostedService<MessageProcessingService>();

        return serviceCollection;
    }

    /// <summary>
    /// Adds dependencies to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> that contains application configuration.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection WithDependencies(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<IEmailSender, EmailSender>();

        DefaultAzureCredential credential = new();

        serviceCollection.AddApplicationInsightsTelemetryWorkerService(options => options.EnableAdaptiveSampling = false);

        serviceCollection.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddServiceBusClientWithNamespace(configuration["AzureServiceBus:Name"]);
            clientBuilder.AddServiceBusAdministrationClientWithNamespace(configuration["AzureServiceBus:Name"]);
            clientBuilder.AddClient<ServiceBusProcessor, ServiceBusProcessorOptions>(
                (_, _, provider) => provider.GetRequiredService<ServiceBusClient>()
                    .CreateProcessor(configuration["AzureServiceBus:TopicNameForEmail"], configuration["AzureServiceBus:SubscriptionName"]))
                    .WithName(EmailTopicProcessor.ProcessorName);

            clientBuilder.UseCredential(credential);
        });

        serviceCollection.AddSingleton<ResourcesInitializer>();

        serviceCollection.AddSingleton<ITopicProcessorFactory, TopicProcessorFactory>();
        serviceCollection.AddSingleton<EmailTopicProcessor>();

        serviceCollection.AddSendGrid(options => options.ApiKey = configuration["SendGrid:ApiKey"]);

        var kcsb = new KustoConnectionStringBuilder(configuration["AzureDataExplorer:HostAddress"], configuration["AzureDataExplorer:DatabaseName"])
            .WithAadTokenProviderAuthentication(async () => (await credential.GetTokenAsync(new(["https://kusto.kusto.windows.net/.default"]))).Token);

        serviceCollection.AddSingleton(KustoClientFactory.CreateCslAdminProvider(kcsb));
        serviceCollection.AddSingleton(KustoIngestFactory.CreateDirectIngestClient(kcsb));
        serviceCollection.AddSingleton<IAzureDataExplorerClient, AzureDataExplorerClient>();

        return serviceCollection;
    }
}
