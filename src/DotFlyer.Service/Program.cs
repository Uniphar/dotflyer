global using Azure.Identity;
global using Azure.Messaging.ServiceBus;
global using Azure.Messaging.ServiceBus.Administration;
global using DotFlyer.Service;
global using DotFlyer.Service.Extensions;
global using DotFlyer.Shared.Payload;
global using Microsoft.ApplicationInsights;
global using Microsoft.Extensions.Azure;
global using SendGrid;
global using SendGrid.Extensions.DependencyInjection;
global using SendGrid.Helpers.Mail;
global using System.Net;
global using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddDotFlyerConfiguration();

builder.Services.AddDotFlyerDependencies(builder.Configuration);
builder.Services.AddSingleton<IMessageProcessor, AzureServiceBusMessageProcessor>();
builder.Services.AddHostedService<MessageProcessingService>();

var host = builder.Build();
host.Run();
