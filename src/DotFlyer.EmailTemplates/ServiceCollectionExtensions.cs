using Microsoft.Extensions.DependencyInjection;

namespace DotFlyer.EmailTemplates
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailTemplates(this IServiceCollection services)
        {
            services.AddSingleton(serviceProvider => new EmailHtmlRenderer(serviceProvider));

            services.AddKeyedSingleton(EmailTemplates.SalesReport, typeof(Components.Templates.SalesReport))
                .AddKeyedSingleton(EmailTemplates.ManualSecretRotation, typeof(Components.Templates.ManualSecretRotation));

            return services;
        }
    }
}
