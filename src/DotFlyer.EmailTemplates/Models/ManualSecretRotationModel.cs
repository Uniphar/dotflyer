namespace DotFlyer.EmailTemplates.Models
{
    public class ManualSecretRotationModel
    {
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceType { get; set; }
        public IEnumerable<string> KeyVaults { get; set; }
        public string SecretName { get; set; }
        public DateTime OldSecretDeletionDateUtc { get; set; }
        public string PwPushUrl { get; set; }
        public int PwPushExpiresInDays { get; set; }
        public int PwPushExpiresAfterViews { get; set; }
    }
}