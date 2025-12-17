using Microsoft.Extensions.DependencyInjection;

namespace DotFlyer.EmailTemplates
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailTemplates(this IServiceCollection services)
        {
            services.AddSingleton(serviceProvider => new EmailHtmlRenderer(serviceProvider));

            services.AddKeyedSingleton(nameof(SalesReportModel), typeof(Components.Templates.SalesReport))
                .AddKeyedSingleton(nameof(ManualSecretRotationModel), typeof(Components.Templates.ManualSecretRotation));

            return services;
        }
    }
}
