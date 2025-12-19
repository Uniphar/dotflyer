namespace DotFlyer.EmailTemplates
{
    /// <summary>
    /// Constants for email template identifiers.
    /// These must match the TemplateId property values in the corresponding model classes.
    /// </summary>
    public static class EmailTemplates
    {
        /// <summary>
        /// Sales report email template.
        /// </summary>
        public const string SalesReport = nameof(SalesReportModel);

        /// <summary>
        /// Manual secret rotation notification email template.
        /// </summary>
        public const string ManualSecretRotation = nameof(ManualSecretRotationModel);
    }
}
