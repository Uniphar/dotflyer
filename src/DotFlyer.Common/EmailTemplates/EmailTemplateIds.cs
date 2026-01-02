namespace DotFlyer.Common.EmailTemplates;

/// <summary>
/// Constants for email template identifiers.
/// These must match the TemplateId property values in the corresponding model classes.
/// </summary>
public static class EmailTemplateIds
{
    /// <summary>
    /// Manual secret rotation notification email template.
    /// </summary>
    public const string ManualSecretRotation = nameof(ManualSecretRotationModel);

    /// <summary>
    /// Manual secret rotation notification email template for Entra App (Service Principal) secrets.
    /// </summary>
    public const string ManualEntraAppSecretRotation = nameof(ManualEntraAppSecretRotationModel);
}