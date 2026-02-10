namespace DotFlyer.Api.Extensions;

/// <summary>
/// Extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds dependencies to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> that contains application configuration.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddDependencies(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        DefaultAzureCredential credential = new();

        serviceCollection.AddSingleton(credential);

        // all telemetry is collected, no sampling
        serviceCollection.AddApplicationInsightsTelemetry(options => options.SamplingRatio = 1.0f);

        serviceCollection.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddServiceBusClientWithNamespace(configuration["AzureServiceBus:Name"]);

            clientBuilder.AddClient<ServiceBusSender, ServiceBusSenderOptions>(
                (_, _, provider) => provider.GetRequiredService<ServiceBusClient>()
                    .CreateSender(configuration["AzureServiceBus:TopicNameForSMS"]))
                    .WithName(SmsTopicSender.Name);

            clientBuilder.AddClient<ServiceBusSender, ServiceBusSenderOptions>(
                (_, _, provider) => provider.GetRequiredService<ServiceBusClient>()
                    .CreateSender(configuration["AzureServiceBus:TopicNameForEmail"]))
                    .WithName(EmailTopicSender.Name);

            clientBuilder.UseCredential(credential);
        });

        serviceCollection.AddSingleton<SmsTopicSender>();
        serviceCollection.AddSingleton<EmailTopicSender>();

        TwilioClient.Init(configuration["Twilio:ApiKeySID"], configuration["Twilio:ApiKeySecret"]);

        return serviceCollection;
    }

    /// <summary>
    /// Adds FluentValidation validators to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddFluentValidators(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IValidator<SMSMessage>, SMSMessageValidator>();
        serviceCollection.AddScoped<IValidator<EmailMessage>, EmailMessageValidator>();

        return serviceCollection;
    }
}
