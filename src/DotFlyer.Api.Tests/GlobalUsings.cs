﻿global using Azure.Identity;
global using Azure.Security.KeyVault.Secrets;
global using DotFlyer.Common.Payload;
global using DotFlyer.Service.AzureDataExplorer.Models;
global using DotFlyer.Service.AzureDataExplorer.Tables;
global using FluentAssertions;
global using Kusto.Cloud.Platform.Data;
global using Kusto.Data;
global using Kusto.Data.Common;
global using Kusto.Data.Net.Client;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Identity.Client;
global using Polly;
global using System.Net;
global using System.Net.Mime;
global using System.Text;
global using System.Text.Json;
