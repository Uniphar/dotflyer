# DotFlyer EmailTemplates

Razor Component Library containing email templates for DotFlyer. It provides reusable Razor components that render HTML email templates using server-side rendering.

## Usage:

1. Create an email message with a template:
   ```csharp
   var emailMessage = new EmailMessage
   {
       Subject = "Manual Secret Rotation",
       TemplateId = EmailTemplateId.ManualSecretRotation,
       TemplateModel = new ManualSecretRotationModel
       {
           Title = "Manual Secret Rotation",
           ResourceName = "AzureKeyVault1",
           SecretName = "secret1"
       }
   };
   ```

1. Send the email:
   ```csharp
   await emailSender.SendAsync(emailMessage);
   ```

The DotFlyer API will render the specified template with the provided model and send the email.

## Available Templates

- `EmailTemplateId.ManualSecretRotation` - Manual secret rotation template

Templates are stored as Razor components in the `Components/` folder.
