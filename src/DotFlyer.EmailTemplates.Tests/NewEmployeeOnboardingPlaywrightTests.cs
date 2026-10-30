namespace DotFlyer.EmailTemplates.Tests;

[TestClass, TestCategory("Playwright")]
public class NewEmployeeOnboardingPlaywrightTests : PlaywrightTestBase
{
    private NewEmployeeOnboardingModel _testModel = null!;
    private EmailMessage _testMessage = null!;

    [TestInitialize]
    public void TestSetup()
    {
        _testModel = new NewEmployeeOnboardingModel
        {
            FirstName = "Jane",
            UserPrincipalName = "jane.doe@example.com",
            PwPushUrl = "https://pwpush.local/p/onboard1",
            PwPushExpiresAfterViews = 5,
            PwPushExpiresInHours = 3,
            Environment = "prod"
        };

        _testMessage = new EmailMessage
        {
            Subject = "Your Microsoft 365 Work Account Is Ready",
            Body = "Fallback body",
            TemplateModel = _testModel,
            TemplateId = EmailTemplateIds.NewEmployeeOnboarding,
            From = new Contact { Email = "sender@test.local", Name = "Sender" },
            To = [new Contact { Email = "to@test.local", Name = "To" }]
        };
    }

    // Cleanup handled by base PlaywrightTestBase.PlaywrightCleanup()

    [TestMethod]
    public async Task NewEmployeeOnboardingEmail_ShouldRenderCorrectly()
    {
        var html = await RenderEmailHtml<NewEmployeeOnboardingModel>(_testMessage);
        await LoadHtml(html);
        await TakeScreenshot("NewEmployeeOnboarding_Full");

        await AssertValidHtmlStructureAsync();
        await AssertRequiredSectionsAsync();
        await AssertLogosAsync();
        await AssertInfoCardAsync();
        await AssertPasswordPushLinkAsync();
        await AssertFooterCompanyInfoAsync();
    }

    private async Task AssertValidHtmlStructureAsync()
    {
        Assert.IsTrue(await ElementExists("html"), "HTML element should exist");
        Assert.IsTrue(await ElementExists("head"), "Head element should exist");
        Assert.IsTrue(await ElementExists("body"), "Body element should exist");
        Assert.IsTrue(await ElementExists("style"), "Style element should exist");
    }

    private async Task AssertRequiredSectionsAsync()
    {
        Assert.IsTrue(await ElementExists(".email-root"), "Email root container should exist");
        Assert.IsTrue(await ElementExists(".email-container"), "Email container should exist");
        Assert.IsTrue(await ElementExists(".brand-bar"), "Brand bar should exist");
        Assert.IsTrue(await ElementExists(".email-header"), "Email header should exist");
        Assert.IsTrue(await ElementExists(".email-body"), "Email body should exist");
        Assert.IsTrue(await ElementExists(".email-footer"), "Email footer should exist");
    }

    private async Task AssertLogosAsync()
    {
        Assert.IsTrue(await ElementExists(".email-header img"), "Header logo should exist");
        Assert.IsTrue(await ElementExists(".email-footer img"), "Footer logo should exist");

        // Verify header logo is visible
        var headerLogo = Page.Locator(".email-header img");
        await headerLogo.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // Verify header logo has loaded by checking naturalWidth
        var headerLogoLoaded = await headerLogo.EvaluateAsync<bool>("img => img.complete && img.naturalWidth > 0");
        Assert.IsTrue(headerLogoLoaded);

        // Verify footer logo element exists
        var footerLogo = Page.Locator(".email-footer img");
        await footerLogo.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // Verify footer logo has loaded by checking naturalWidth
        var footerLogoLoaded = await footerLogo.EvaluateAsync<bool>("img => img.complete && img.naturalWidth > 0");
        Assert.IsTrue(footerLogoLoaded);
    }

    private async Task AssertInfoCardAsync()
    {
        Assert.IsTrue(await ElementExists(".info-card"), "Info card should exist");
        Assert.IsTrue(await ElementExists(".info-card-title"), "Info card title should exist");

        var bodyText = await GetElementText(".email-body");
        Assert.IsNotNull(bodyText);
        StringAssert.Contains(bodyText, _testModel.FirstName);
        StringAssert.Contains(bodyText, _testModel.UserPrincipalName);
    }

    private async Task AssertPasswordPushLinkAsync()
    {
        var link = Page.Locator($"a[href='{_testModel.PwPushUrl}']");
        Assert.IsGreaterThan(0, await link.CountAsync(), "Password push link should exist");

        var deliveryLabel = Page.GetByText("Temporary passcode delivery link:", new PageGetByTextOptions { Exact = true });
        await deliveryLabel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        var signInUrl = $"https://mysignins.microsoft.com/security-info?login_hint={Uri.EscapeDataString(_testModel.UserPrincipalName)}";
        var signInLink = Page.Locator($"a[href='{signInUrl}']");
        Assert.IsGreaterThan(0, await signInLink.CountAsync(), "Microsoft 365 sign-in link should exist");

        var bodyText = await GetElementText(".email-body");
        Assert.IsNotNull(bodyText);
        StringAssert.Contains(bodyText, "Temporary passcode delivery link:");
        StringAssert.Contains(bodyText, $"{_testModel.PwPushExpiresInHours} hours");
        StringAssert.Contains(bodyText, _testModel.PwPushExpiresAfterViews.ToString());
    }

    private async Task AssertFooterCompanyInfoAsync()
    {
        Assert.IsTrue(await ElementExists(".footer-address"), "Footer address should exist");
        var footerText = await GetElementText(".email-footer");
        Assert.IsNotNull(footerText);
        StringAssert.Contains(footerText, "Uniphar");
        StringAssert.Contains(footerText, "Kingswood Road");
    }
}
