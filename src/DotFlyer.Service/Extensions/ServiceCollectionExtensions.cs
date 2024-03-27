namespace DotFlyer.Service.Extensions;

/// <summary>
/// Extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds dependencies to the service collection of DotFlyer application.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> that contains application configuration.</param>
    public static void AddDotFlyerDependencies(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddApplicationInsightsTelemetryWorkerService(options => options.EnableAdaptiveSampling = false);

        serviceCollection.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddServiceBusClientWithNamespace(configuration["AzureServiceBus:Name"]);
            clientBuilder.AddServiceBusAdministrationClientWithNamespace(configuration["AzureServiceBus:Name"]);
            clientBuilder.AddClient<ServiceBusProcessor, ServiceBusProcessorOptions>(
                (_, _, provider) => provider.GetRequiredService<ServiceBusClient>()
                    .CreateProcessor(configuration["ServiceBus:TopicName"], configuration["ServiceBus:SubscriptionName"]));

            clientBuilder.UseCredential(new DefaultAzureCredential());
        });

        serviceCollection.AddSendGrid(options => options.ApiKey = configuration["SendGrid:ApiKey"]);
    }
}
