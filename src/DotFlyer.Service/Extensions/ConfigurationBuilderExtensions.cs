namespace DotFlyer.Service.Extensions;

/// <summary>
/// Extension methods for the <see cref="IConfigurationBuilder"/> interface.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="environment">The environment name, one of dev, test or prod.</param>
    /// <returns></returns>
    public static IConfigurationBuilder AddDotFlyerConfiguration(this IConfigurationBuilder configurationBuilder, string environment)
    {
        return configurationBuilder.AddUniOldKeyvault(environment, new DefaultAzureCredential());
    }
}
