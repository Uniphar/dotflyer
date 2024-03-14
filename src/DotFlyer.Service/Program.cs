global using Azure.Identity;
global using Azure.Messaging.ServiceBus;
global using Azure.Messaging.ServiceBus.Administration;
global using DotFlyer.Service;
global using DotFlyer.Shared.Payload;
global using Microsoft.Extensions.Azure;
global using SendGrid;
global using SendGrid.Extensions.DependencyInjection;
global using SendGrid.Helpers.Mail;
global using System.Net;
global using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{Environment.GetEnvironmentVariable("KEY_VAULT_NAME")}.vault.azure.net/"),
    new DefaultAzureCredential());

builder.Services.AddApplicationInsightsTelemetryWorkerService(options => options.EnableAdaptiveSampling = false);

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddServiceBusClientWithNamespace(builder.Configuration["AzureServiceBus:Name"]);
    clientBuilder.AddServiceBusAdministrationClientWithNamespace(builder.Configuration["AzureServiceBus:Name"]);
    clientBuilder.AddClient<ServiceBusProcessor, ServiceBusProcessorOptions>(
        (_, _, provider) => provider.GetRequiredService<ServiceBusClient>()
            .CreateProcessor(builder.Configuration["ServiceBus:TopicName"], builder.Configuration["ServiceBus:SubscriptionName"]));

    clientBuilder.UseCredential(new DefaultAzureCredential());
});

builder.Services.AddSendGrid(options => options.ApiKey = builder.Configuration["SendGrid:ApiKey"]);

builder.Services.AddHostedService<MessageProcessor>();

var host = builder.Build();
host.Run();
