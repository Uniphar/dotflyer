using DotFlyer.Common.EmailTemplates;
using DotFlyer.Common.Payload;

namespace DotFlyer.EmailTemplates.Tests;

[TestClass, TestCategory("Playwright")]
public class ManualSecretRotationPlaywrightTests : PlaywrightTestBase
{
    private ManualSecretRotationModel _testModel = null!;
    private EmailMessage _testMessage = null!;

    [TestInitialize]
    public async Task TestSetup()
    {
        await PlaywrightSetup();
        
        _testModel = new ManualSecretRotationModel
        {
            SecretName = "DbPassword",
            TenantId = Guid.NewGuid().ToString(),
            AppId = Guid.NewGuid().ToString(),
            ResourceName = "sqlserver1",
            KeyVaults = new List<string>
            {
                "https://kv1.vault.local",
                "https://kv2.vault.local",
                "https://kv3.vault.local"
            },
            OldSecretDeletionDateUtc = DateTime.UtcNow.AddDays(7),
            PwPushUrl = "https://pwpush.local/p/abcdefg",
            PwPushExpiresAfterViews = 5,
            PwPushExpiresInDays = 3
        };

        _testMessage = new EmailMessage
        {
            Subject = "Manual Secret Rotation Required",
            Body = "Fallback body",
            TemplateModel = _testModel,
            TemplateId = EmailTemplateIds.ManualSecretRotation,
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
    public async Task EmailLayout_ShouldHaveValidHtmlStructure()
    {
        var html = await RenderEmailHtml<ManualSecretRotationModel>(_testMessage);
        await LoadHtml(html);
        await TakeScreenshot("ManualSecretRotation_Full");

        Assert.IsTrue(await ElementExists("html"), "HTML element should exist");
        Assert.IsTrue(await ElementExists("head"), "Head element should exist");
        Assert.IsTrue(await ElementExists("body"), "Body element should exist");
        Assert.IsTrue(await ElementExists("style"), "Style element should exist");
    }

    [TestMethod]
    public async Task EmailLayout_ShouldContainAllRequiredSections()
    {
        var html = await RenderEmailHtml<ManualSecretRotationModel>(_testMessage);
        await LoadHtml(html);

        Assert.IsTrue(await ElementExists(".email-root"), "Email root container should exist");
        Assert.IsTrue(await ElementExists(".email-container"), "Email container should exist");
        Assert.IsTrue(await ElementExists(".brand-bar"), "Brand bar should exist");
        Assert.IsTrue(await ElementExists(".email-header"), "Email header should exist");
        Assert.IsTrue(await ElementExists(".email-body"), "Email body should exist");
        Assert.IsTrue(await ElementExists(".email-footer"), "Email footer should exist");
    }

    [TestMethod]
    public async Task EmailLayout_ShouldDisplayLogoInHeaderAndFooter()
    {
        var html = await RenderEmailHtml<ManualSecretRotationModel>(_testMessage);
        await LoadHtml(html);

        Assert.IsTrue(await ElementExists(".email-header img"), "Header logo should exist");
        Assert.IsTrue(await ElementExists(".email-footer img"), "Footer logo should exist");
    }

    [TestMethod]
    public async Task EmailContent_ShouldDisplayRotationDetails()
    {
        var html = await RenderEmailHtml<ManualSecretRotationModel>(_testMessage);
        await LoadHtml(html);

        Assert.IsTrue(await ElementExists(".info-card"), "Info card should exist");
        Assert.IsTrue(await ElementExists(".info-card-title"), "Info card title should exist");

        var bodyText = await GetElementText(".email-body");
        Assert.IsNotNull(bodyText);
        StringAssert.Contains(bodyText, _testModel.TenantId);
        StringAssert.Contains(bodyText, _testModel.AppId);
        StringAssert.Contains(bodyText, _testModel.SecretName);
    }

    [TestMethod]
    public async Task EmailContent_ShouldDisplayAllKeyVaults()
    {
        var html = await RenderEmailHtml<ManualSecretRotationModel>(_testMessage);
        await LoadHtml(html);

        Assert.IsTrue(await ElementExists(".info-card ul"), "Key vaults list should exist");
        
        var listItemsCount = await Page.Locator(".info-card ul li").CountAsync();
        var expectedCount = _testModel.KeyVaults?.Count();
        Assert.AreEqual(expectedCount, listItemsCount, 
            $"Should display all {expectedCount} key vaults");

        foreach (var kv in _testModel.KeyVaults!)
        {
            var bodyText = await GetElementText(".email-body");
            StringAssert.Contains(bodyText, kv, $"Key vault {kv} should be displayed");
        }
    }

    [TestMethod]
    public async Task EmailContent_ShouldDisplayPasswordPushLink()
    {
        var html = await RenderEmailHtml<ManualSecretRotationModel>(_testMessage);
        await LoadHtml(html);

        var link = Page.Locator($"a[href='{_testModel.PwPushUrl}']");
        Assert.IsGreaterThan(0, await link.CountAsync(), "Password push link should exist");
        
        var bodyText = await GetElementText(".email-body");
        Assert.IsNotNull(bodyText);
        StringAssert.Contains(bodyText, _testModel.PwPushExpiresInDays.ToString());
        StringAssert.Contains(bodyText, _testModel.PwPushExpiresAfterViews.ToString());
    }

    [TestMethod]
    public async Task EmailFooter_ShouldDisplayCompanyInfo()
    {
        var html = await RenderEmailHtml<ManualSecretRotationModel>(_testMessage);
        await LoadHtml(html);

        Assert.IsTrue(await ElementExists(".footer-address"), "Footer address should exist");
        var footerText = await GetElementText(".email-footer");
        Assert.IsNotNull(footerText);
        StringAssert.Contains(footerText, "Uniphar");
        StringAssert.Contains(footerText, "Kingswood Road");
    }
}
