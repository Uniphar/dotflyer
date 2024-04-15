global using Azure.Identity;
global using Azure.Messaging.ServiceBus;
global using Azure.Messaging.ServiceBus.Administration;
global using DotFlyer.Service;
global using DotFlyer.Service.Extensions;
global using DotFlyer.Shared.Payload;
global using Microsoft.Extensions.Azure;
global using SendGrid;
global using SendGrid.Extensions.DependencyInjection;
global using SendGrid.Helpers.Mail;
global using System.Net;
global using System.Text.Json;

var builder = Host.CreateApplicationBuilder();

builder.Configuration
    .AddDotFlyerConfiguration();

builder.Services
    .AddDotFlyerMessageProcessor<AzureServiceBusMessageProcessor>()
    .WithEmailSender<SendGridEmailSender>()
    .WithDependencies(builder.Configuration);

var host = builder.Build();
host.Run();
