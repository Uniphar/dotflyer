# DotFlyer EmailTemplates

Razor Class Library containing email templates for DotFlyer. 
It expects templates to be stored in `Views/Email` folder and provides a minimal `TemplateRenderer` which uses Razor engine.

## Usage:

1. Add a project reference to `DotFlyer.EmailTemplates`.
2. In your ASP.NET Core host, you can render a view to string using `IRazorViewEngine` or a small helper. Example helper usage:

   ```csharp
   public async Task<string> RenderEmailAsync(EmailMessage model, IViewRenderService viewRenderService)
   {
       // view path is "~/Views/Email/EmailMessage.cshtml"
       return await viewRenderService.RenderToStringAsync("/Views/Email/EmailMessage.cshtml", model);
   }
   ```

Minimal templates are stored in `Views/Email/EmailMessage.cshtml`.
