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
        where TMessageProcessor : class, IMessageProcessor
    {
        serviceCollection.AddSingleton<IMessageProcessor, TMessageProcessor>();
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
        DefaultAzureCredential credential = new();

        serviceCollection.AddApplicationInsightsTelemetryWorkerService(options => options.EnableAdaptiveSampling = false);

        serviceCollection.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddServiceBusClientWithNamespace(configuration["AzureServiceBus:Name"]);
            clientBuilder.AddServiceBusAdministrationClientWithNamespace(configuration["AzureServiceBus:Name"]);
            clientBuilder.AddClient<ServiceBusProcessor, ServiceBusProcessorOptions>(
                (_, _, provider) => provider.GetRequiredService<ServiceBusClient>()
                    .CreateProcessor(configuration["AzureServiceBus:TopicName"], configuration["AzureServiceBus:SubscriptionName"]));

            clientBuilder.UseCredential(credential);
        });

        serviceCollection.AddSendGrid(options => options.ApiKey = configuration["SendGrid:ApiKey"]);

        var kcsb = new KustoConnectionStringBuilder(configuration["AzureDataExplorer:HostAddress"], configuration["AzureDataExplorer:DatabaseName"])
            .WithAadApplicationTokenAuthentication(credential.GetToken(new(["https://kusto.kusto.windows.net/.default"])).Token);

        serviceCollection.AddSingleton(KustoClientFactory.CreateCslAdminProvider(kcsb));
        serviceCollection.AddSingleton(KustoIngestFactory.CreateDirectIngestClient(kcsb));
        serviceCollection.AddSingleton<AzureDataExplorerClient>();

        return serviceCollection;
    }

    /// <summary>
    /// Adds an email sender to the service collection.
    /// </summary>
    /// <typeparam name="TEmailSender"></typeparam>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection WithEmailSender<TEmailSender>(this IServiceCollection serviceCollection)
        where TEmailSender : class, IEmailSender
    {
        serviceCollection.AddSingleton<IEmailSender, TEmailSender>();

        return serviceCollection;
    }
}
