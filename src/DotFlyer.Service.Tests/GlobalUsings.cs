﻿global using Azure.Identity;
global using Azure.Messaging.ServiceBus;
global using Azure.Security.KeyVault.Secrets;
global using DotFlyer.Common.Payload;
global using DotFlyer.Service.AzureDataExplorer;
global using DotFlyer.Service.AzureDataExplorer.Models;
global using DotFlyer.Service.AzureDataExplorer.Tables;
global using DotFlyer.Service.Senders;
global using FluentAssertions;
global using Kusto.Cloud.Platform.Data;
global using Kusto.Data;
global using Kusto.Data.Common;
global using Kusto.Data.Net.Client;
global using Microsoft.ApplicationInsights;
global using Microsoft.ApplicationInsights.Channel;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using Polly;
global using SendGrid;
global using SendGrid.Helpers.Mail;
global using System.Net;
global using System.Text;
global using System.Text.Json;
global using Twilio.Clients;
global using Twilio.Http;
