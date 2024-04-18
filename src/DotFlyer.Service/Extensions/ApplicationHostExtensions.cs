namespace DotFlyer.Service.Extensions;

public static class ApplicationHostExtensions
{
    public static async Task<IHost> InitializeResourcesAsync(this IHost app)
    {
        await app.Services.GetRequiredService<ResourcesInitializer>().InitializeAsync();

        return app;
    }
}
