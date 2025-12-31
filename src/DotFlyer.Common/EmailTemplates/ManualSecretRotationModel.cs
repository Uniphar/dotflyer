using System.Collections.Generic;

namespace DotFlyer.Common.EmailTemplates;

public class ManualSecretRotationModel
{
    public required string TenantId { get; init; }
    public required string AppId { get; init; }
    public required string ResourceName { get; init; }
    public IEnumerable<string>? KeyVaults { get; set; }
    public required string SecretName { get; init; }
    public DateTime OldSecretDeletionDateUtc { get; set; }
    public string? PwPushUrl { get; set; }
    public int PwPushExpiresInDays { get; set; }
    public int PwPushExpiresAfterViews { get; set; }
}