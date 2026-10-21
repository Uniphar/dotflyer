namespace DotFlyer.Common.EmailTemplates;

/// <summary>
/// Model for new employee onboarding email templates.
/// </summary>
public class NewEmployeeOnboardingModel
{
    public required string FirstName { get; init; }
    public required string UserPrincipalName { get; init; }
    public string? PwPushUrl { get; set; }
    public int PwPushExpiresInDays { get; set; }
    public int PwPushExpiresAfterViews { get; set; }

    public string? Environment { get; set; }
}
