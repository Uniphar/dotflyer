namespace DotFlyer.EmailTemplates
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailTemplates(this IServiceCollection services)
        {
            services.AddSingleton(serviceProvider => new EmailHtmlRenderer(
                serviceProvider,
                serviceProvider.GetRequiredService<ILogger<EmailHtmlRenderer>>()));

            services.AddKeyedSingleton(EmailTemplateIds.ManualSecretRotation, typeof(Components.ManualSecretRotation));

            return services;
        }
    }
}
