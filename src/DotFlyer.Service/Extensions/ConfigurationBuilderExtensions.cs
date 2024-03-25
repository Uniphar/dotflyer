namespace DotFlyer.Service.Extensions;

/// <summary>
/// Extension methods for the <see cref="IConfigurationBuilder"/> interface.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    public static void AddDotFlyerConfiguration(this IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddAzureKeyVault(
            new Uri($"https://{Environment.GetEnvironmentVariable("KEY_VAULT_NAME")}.vault.azure.net/"),
            new DefaultAzureCredential());
    }
}
