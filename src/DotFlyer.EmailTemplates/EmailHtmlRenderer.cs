using DotFlyer.Common.Payload;
using DotFlyer.EmailTemplates.Components.Email;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotFlyer.EmailTemplates
{
    public class EmailHtmlRenderer(IServiceProvider serviceProvider)
    {
        public async Task<string> RenderAsync(EmailMessage model)
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

            var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var dictionary = new Dictionary<string, object?>
                {
                    ["Model"] = model
                };

                var parameters = ParameterView.FromDictionary(dictionary);
                var output = await htmlRenderer.RenderComponentAsync<EmailTemplate>(parameters);

                return output.ToHtmlString();
            });
            return html;
        }
    }
}
