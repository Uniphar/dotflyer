global using Azure.Identity;
global using Azure.Messaging.ServiceBus;
global using Azure.Messaging.ServiceBus.Administration;
global using Azure.Storage.Blobs;
global using DotFlyer.Common.Payload;
global using DotFlyer.Service;
global using DotFlyer.Service.AzureDataExplorer;
global using DotFlyer.Service.AzureDataExplorer.Models;
global using DotFlyer.Service.AzureDataExplorer.Tables;
global using DotFlyer.Service.Extensions;
global using DotFlyer.Service.Senders;
global using DotFlyer.Service.TopicProcessors;
global using Kusto.Data;
global using Kusto.Data.Common;
global using Kusto.Data.Ingestion;
global using Kusto.Data.Net.Client;
global using Kusto.Ingest;
global using Kusto.Ingest.Exceptions;
global using Microsoft.Extensions.Azure;
global using SendGrid;
global using SendGrid.Extensions.DependencyInjection;
global using SendGrid.Helpers.Mail;
global using System.Net;
global using System.Text.Json;
global using Twilio;
global using Twilio.Clients;
global using Twilio.Http;
global using Twilio.Rest.Api.V2010.Account;
global using Twilio.Types;

var builder = Host.CreateApplicationBuilder();

builder.Configuration
    .AddDotFlyerConfiguration();

builder.Services
    .AddDotFlyerMessagesProcessor<AzureServiceBusMessagesProcessor>()
    .WithDependencies(builder.Configuration);

var host = builder.Build();

await host.InitializeResourcesAsync();

host.Run();
