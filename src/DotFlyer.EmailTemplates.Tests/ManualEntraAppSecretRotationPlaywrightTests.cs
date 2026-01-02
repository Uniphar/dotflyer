using DotFlyer.Common.EmailTemplates;
using DotFlyer.Common.Payload;
using Microsoft.Playwright;

namespace DotFlyer.EmailTemplates.Tests;

[TestClass, TestCategory("Playwright")]
public class ManualEntraAppSecretRotationPlaywrightTests : PlaywrightTestBase
{
    private ManualEntraAppSecretRotationModel _testModel = null!;
    private EmailMessage _testMessage = null!;

    [TestInitialize]
    public async Task TestSetup()
    {
        await PlaywrightSetup();

        _testModel = new ManualEntraAppSecretRotationModel
        {
            TenantId = Guid.NewGuid().ToString(),
            AppId = Guid.NewGuid().ToString(),
            SecretName = "ServicePrincipalSecret",
            ResourceName = "my-service-principal",
            KeyVaults = new List<string>
            {
                "https://kv1.vault.local",
                "https://kv2.vault.local",
                "https://kv3.vault.local"
            },
            OldSecretDeletionDateUtc = DateTime.UtcNow.AddDays(7),
            PwPushUrl = "https://pwpush.local/p/xyz789",
            PwPushExpiresAfterViews = 5,
            PwPushExpiresInDays = 3
        };

        _testMessage = new EmailMessage
        {
            Subject = "Manual Entra App Secret Rotation Required",
            Body = "Fallback body",
            TemplateModel = _testModel,
            TemplateId = EmailTemplateIds.ManualEntraAppSecretRotation,
            From = new Contact { Email = "sender@test.local", Name = "Sender" },
            To = [new Contact { Email = "to@test.local", Name = "To" }]
        };
    }

    [TestCleanup]
    public async Task TestTeardown()
    {
        await PlaywrightCleanup();
    }

    [TestMethod]
    public async Task ManualEntraAppSecretRotationEmail_ShouldRenderCorrectly()
    {
        var html = await RenderEmailHtml<ManualEntraAppSecretRotationModel>(_testMessage);
        await LoadHtml(html);
        await TakeScreenshot("ManualEntraAppSecretRotation_Full");

        await AssertValidHtmlStructureAsync();
        await AssertRequiredSectionsAsync();
        await AssertLogosAsync();
        await AssertRotationDetailsAsync();
        await AssertKeyVaultsAsync();
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

    private async Task AssertRotationDetailsAsync()
    {
        Assert.IsTrue(await ElementExists(".info-card"), "Info card should exist");
        Assert.IsTrue(await ElementExists(".info-card-title"), "Info card title should exist");

        var bodyText = await GetElementText(".email-body");
        Assert.IsNotNull(bodyText);
        StringAssert.Contains(bodyText, _testModel.TenantId);
        StringAssert.Contains(bodyText, _testModel.AppId);
        StringAssert.Contains(bodyText, _testModel.SecretName);
    }

    private async Task AssertKeyVaultsAsync()
    {
        Assert.IsTrue(await ElementExists(".info-card ul"), "Key vaults list should exist");

        var listItemsCount = await Page.Locator(".info-card ul li").CountAsync();
        var expectedCount = _testModel.KeyVaults?.Count();
        Assert.AreEqual(expectedCount, listItemsCount,
            $"Should display all {expectedCount} key vaults");

        var bodyText = await GetElementText(".email-body");
        Assert.IsNotNull(bodyText);
        foreach (var kv in _testModel.KeyVaults!)
        {
            StringAssert.Contains(bodyText, kv, $"Key vault {kv} should be displayed");
        }
    }

    private async Task AssertPasswordPushLinkAsync()
    {
        var link = Page.Locator($"a[href='{_testModel.PwPushUrl}']");
        Assert.IsGreaterThan(0, await link.CountAsync(), "Password push link should exist");

        var bodyText = await GetElementText(".email-body");
        Assert.IsNotNull(bodyText);
        StringAssert.Contains(bodyText, _testModel.PwPushExpiresInDays.ToString());
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
