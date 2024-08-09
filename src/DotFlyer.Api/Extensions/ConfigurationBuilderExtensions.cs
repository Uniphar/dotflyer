namespace DotFlyer.Api.Extensions;

/// <summary>
/// Extension methods for the <see cref="IConfigurationBuilder"/> interface.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddDotFlyerConfiguration(this IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddAzureKeyVault(
            new Uri($"https://{Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_NAME")}.vault.azure.net/"),
            new DefaultAzureCredential());

        return configurationBuilder;
    }
}
