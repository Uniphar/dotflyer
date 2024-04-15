# DotFlyer

DotFlyer is a .NET application that provides SMS and email sending capabilities. It abstracts the underlying message services, like Twilio and SendGrid, and provides a unified interface to send messages via message broker or API.

The solution consists of the following projects:

- DotFlyer.Service: The worker project that listens to the message broker and sends SMS or emails based on the data in the message.
- DotFlyer.Api: The API project that provides endpoints to send messages into the message broker.

## Overview

### DotFlyer.Service

DotFlyer.Service is a .NET [worker service](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services) that runs the custom implementation of the `IMessageProcessor` interface.

The configuration of the service is driven by settings loaded via `AddDotFlyerConfiguration` extension method, where any required configuration source can be added. The service uses `appsettings.json` file (loaded by default) and Azure KeyVault for storing secrets and configuration settings. `AZURE_KEY_VAULT_NAME` environment variable specifies the name of the Azure KeyVault resource.

 Azure Service Bus is used in the solution as a message broker. `AzureServiceBusMessageProcessor` class implements the `IMessageProcessor` interface and is added to the service collection by `AddDotFlyerMessageProcessor<TMessageProcessor>` extension method.

 Email sender is abstracted by `IEmailSender` interface, which can be implemented and added to the service collection by `AddDotFlyerEmailSender<TEmailSender>` extension method and used in `IMessageProcessor` implementation as a dependency.

All required dependencies/services can be added with `WithDependencies` extension method providing `IConfiguration` instance for configuration setup. The service uses Application Insights for telemetry and logging, Azure Service Bus clients for topic/subscription setup and receiving messages, and SendGrid client for sending emails.

The following configuration settings are required by the service:

- `AzureServiceBus:Name` - Azure Service Bus namespace name
- `AzureServiceBus:TopicName` - Azure Service Bus topic name, where messages are sent
- `AzureServiceBus:SubscriptionName` - Azure Service Bus subscription name, where messages are received
- `SendGrid:ApiKey` - SendGrid API key for sending emails
