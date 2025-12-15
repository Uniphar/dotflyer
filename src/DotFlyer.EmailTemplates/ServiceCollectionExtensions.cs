using System;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotFlyer.EmailTemplates
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailTemplates(this IServiceCollection services)
        {
            services.AddSingleton<EmailHtmlRenderer>(serviceProvider =>
            {
                return new EmailHtmlRenderer(serviceProvider);
            });

            return services;
        }
    }
}
