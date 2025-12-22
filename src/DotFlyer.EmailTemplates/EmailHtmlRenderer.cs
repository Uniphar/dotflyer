using System.Text.Json;
using System.Reflection;

namespace DotFlyer.EmailTemplates
{
    public class EmailHtmlRenderer(IServiceProvider serviceProvider, ILogger<EmailHtmlRenderer> logger)
    {
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
            if (emailMessage.TemplateId == null)
            {
                return emailMessage.Body;
            }

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

            // If a template id is provided, try to resolve a registered component for it.
            try
            {
                var componentType = serviceProvider.GetKeyedService<Type>(emailMessage.TemplateId);
                if (componentType == null)
                {
                    throw new InvalidOperationException($"No component registered for template id {emailMessage.TemplateId}");
                }

                var model = emailMessage.TemplateModel;

                // Deserialize the model if it's a JsonElement (from API requests)
                if (model is JsonElement jsonElement)
                {
                    // Get the model type from the component's Model parameter
                    var modelType = componentType.GetProperty("Model")?.PropertyType;
                    if (modelType == null)
                    {
                        throw new InvalidOperationException($"Component {componentType.FullName} does not have a required Model property");
                    }
                    model = JsonSerializer.Deserialize(jsonElement.GetRawText(), modelType);
                }
                
                var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
                {
                    ["Model"] = model
                });

                var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
                {
                    var output = await htmlRenderer.RenderComponentAsync(componentType, parameters);
                    return output.ToHtmlString();
                });

                return html;
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "No keyed service found for model type {TemplateId}, falling back to email body", emailMessage.TemplateId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to render email template for template id {TemplateId}", emailMessage.TemplateId);
            }

            //if failed to render html template, fall back to json serialized model
            return JsonSerializer.Serialize(emailMessage.TemplateModel);
        }
    }
}
