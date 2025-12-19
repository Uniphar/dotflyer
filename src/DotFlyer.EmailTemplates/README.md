# DotFlyer EmailTemplates

Razor Component Library containing email templates for DotFlyer. It provides reusable Razor components that render HTML email templates using server-side rendering.

## Usage:

1. Add the NuGet package to your project:
   ```bash
   dotnet add package DotFlyer.EmailTemplates
   ```

1. Create an email message with a template:
   ```csharp
   var emailMessage = new EmailMessage
   {
       Subject = "Sales Report",
       TemplateId = EmailTemplateId.SalesReport,
       TemplateModel = new SalesReportModel
       {
           Title = "Q1 Sales Report",
           ClientName = "Acme Corp",
           ContactEmailAddress = "contact@acme.com"
       }
   };
   ```

1. Send the email:
   ```csharp
   await emailSender.SendAsync(emailMessage);
   ```

The DotFlyer API will render the specified template with the provided model and send the email.

## Available Templates

- `EmailTemplateId.SalesReport` - Sales report template
- `EmailTemplateId.ManualSecretRotation` - Manual secret rotation template

Templates are stored as Razor components in the `Components/` folder.
