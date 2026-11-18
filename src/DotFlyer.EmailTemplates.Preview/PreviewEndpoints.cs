using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DotFlyer.EmailTemplates.Preview;

public static class PreviewEndpoints
{
    private const string HtmlContentType = "text/html; charset=utf-8";

    public static void MapPreviewEndpoints(this WebApplication app)
    {
        app.MapGet("/preview/{name}", async (string name, TemplateCatalog catalog, HttpContext context) =>
        {
            if (!catalog.TryGet(name, out var template))
            {
                return Results.NotFound();
            }

            var model = catalog.CreateModel(template.ModelType);
            var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
            await using var renderer = new HtmlRenderer(context.RequestServices, loggerFactory);
            var html = await renderer.Dispatcher.InvokeAsync(async () =>
            {
                var output = await renderer.RenderComponentAsync(template.ComponentType, ParameterView.FromDictionary(new Dictionary<string, object?> { ["Model"] = model }));
                return output.ToHtmlString();
            });
            return Results.Content(html, HtmlContentType);
        });
    }
}
