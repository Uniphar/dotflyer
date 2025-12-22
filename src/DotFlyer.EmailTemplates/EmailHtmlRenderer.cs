using DotFlyer.Common.Payload;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotFlyer.EmailTemplates
{
    public class EmailHtmlRenderer(IServiceProvider serviceProvider)
    {
        private readonly ILogger<EmailHtmlRenderer> _logger = serviceProvider.GetRequiredService<ILogger<EmailHtmlRenderer>>();

        /// <summary>
        /// Renders an email message to HTML using a Razor component template or falls back to the email body.
        /// </summary>
        /// <remarks>
        /// If the email message has a TemplateModel, the method attempts to resolve and render a registered Razor component
        /// using the TemplateId as a keyed service. If no template is found or an error occurs during rendering,
        /// it falls back to returning the Body property of the email message.
        /// </remarks>
        public async Task<string?> RenderAsync(EmailMessage emailMessage)
        {
            // No template, use Body instead (legacy behavior).
            if (emailMessage.TemplateModel == null)
            {
                return emailMessage.Body;
            }

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

            // If a template id is provided, try to resolve a registered component for it.
            try
            {
                var componentType = serviceProvider.GetKeyedService<Type>(emailMessage.TemplateId);
                if (componentType != null)
                {
                    var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
                    {
                        ["Model"] = emailMessage.TemplateModel
                    });

                    var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
                    {
                        var output = await htmlRenderer.RenderComponentAsync(componentType, parameters);
                        return output.ToHtmlString();
                    });

                    return html;
                }

                _logger.LogWarning("No component registered for model type {TemplateId}, falling back to email body", emailMessage.TemplateId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "No keyed service found for model type {TemplateId}, falling back to email body", emailMessage.TemplateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while rendering email template for model type {TemplateId}, falling back to email body", emailMessage.TemplateId);
            }

            return emailMessage.Body;
        }
    }
}
