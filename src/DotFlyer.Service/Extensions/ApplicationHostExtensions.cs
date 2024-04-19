namespace DotFlyer.Service.Extensions;

/// <summary>
/// Application host extensions.
/// </summary>
public static class ApplicationHostExtensions
{
    /// <summary>
    /// Initializes resources needed by the application.
    /// </summary>
    /// <param name="app">The <see cref="IHost"/> instance.</param>
    /// <returns>The <see cref="IHost"/> instance.</returns>
    public static async Task<IHost> InitializeResourcesAsync(this IHost app)
    {
        await app.Services.GetRequiredService<ResourcesInitializer>().InitializeAsync();

        return app;
    }
}
