using System;
using System.Collections.Generic;

namespace DotFlyer.Common.EmailTemplates
{
    public class ManualSecretRotationModel
    {
        public string? TenantId { get; set; }
        public string? AppId { get; set; }
        public string? ResourceName { get; set; }
        public IEnumerable<string>? KeyVaults { get; set; }
        public string? SecretName { get; set; }
        public DateTime OldSecretDeletionDateUtc { get; set; }
        public string? PwPushUrl { get; set; }
        public int PwPushExpiresInDays { get; set; }
        public int PwPushExpiresAfterViews { get; set; }
    }
}