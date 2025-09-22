using Azure.Extensions.AspNetCore.Configuration.Secrets;

namespace DotFlyer.Api.Extensions;

/// <summary>
/// Extension methods for the <see cref="ConfigurationManager"/> class.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// Adds the Azure Key Vault configuration provider to the configuration manager and configures authentication options.
    /// </summary>
    /// <param name="configurationManager">The <see cref="ConfigurationManager"/>.</param>
    public static void AddDotFlyerConfiguration(this ConfigurationManager configurationManager)
    {
        configurationManager.AddAzureKeyVault(
            new Uri($"https://{Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_NAME")}.vault.azure.net/"),
            new DefaultAzureCredential(), new AzureKeyVaultConfigurationOptions
            {
                ReloadInterval = TimeSpan.FromMinutes(20)
            });

        Dictionary<string, string?> azureAuthConfiguration = new()
        {
            { "AzureAd:Instance", "https://login.microsoftonline.com/" },
            { "AzureAd:TenantId", Environment.GetEnvironmentVariable("AZURE_ENTRA_EXTERNAL_TENANT_ID") },
            { "AzureAd:ClientId", configurationManager["dotflyer-api-client-id"] },
            { "AzureAd:Audience", configurationManager["dotflyer-api-client-id"] }
        };

        configurationManager.AddInMemoryCollection(azureAuthConfiguration);
    }
}
