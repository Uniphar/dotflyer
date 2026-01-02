using System.Collections.Generic;

namespace DotFlyer.Common.EmailTemplates;

/// <summary>
/// Model for manual secret rotation email templates.
/// </summary>
public class ManualSecretRotationModel
{
    public required string ResourceName { get; init; }
    public required IEnumerable<string> KeyVaults { get; init; }
    public required string SecretName { get; init; }
    public DateTime OldSecretDeletionDateUtc { get; set; }
    public string? PwPushUrl { get; set; }
    public int PwPushExpiresInDays { get; set; }
    public int PwPushExpiresAfterViews { get; set; }
}

/// <summary>
/// Model for Entra App (service principal) secret rotation notifications.
/// Extends the base model with Entra-specific tenant and application identifiers.
/// </summary>
public class ManualEntraAppSecretRotationModel : ManualSecretRotationModel
{
    public required string TenantId { get; init; }
    public required string AppId { get; init; }
}
