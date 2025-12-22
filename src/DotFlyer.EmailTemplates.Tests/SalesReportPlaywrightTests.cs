using DotFlyer.Common.EmailTemplates;
using DotFlyer.Common.Payload;

namespace DotFlyer.EmailTemplates.Tests;

[TestClass, TestCategory("Playwright")]
public class SalesReportPlaywrightTests : PlaywrightTestBase
{
    private SalesReportModel _testModel = null!;
    private EmailMessage _testMessage = null!;

    [TestInitialize]
    public async Task TestSetup()
    {
        await PlaywrightSetup();
        
        _testModel = new SalesReportModel
        {
            Title = "Sales Report - January 2024",
            ClientName = "Test Corporation",
            ContactEmailAddress = "support@testcorp.com"
        };

        _testMessage = new EmailMessage
        {
            Subject = "Monthly Sales Report",
            Body = "Fallback body",
            TemplateModel = _testModel,
            TemplateId = EmailTemplateIds.SalesReport,
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
        var html = await RenderEmailHtml<SalesReportModel>(_testMessage);
        await LoadHtml(html);
        await TakeScreenshot("SalesReport_Full");

        Assert.IsTrue(await ElementExists("html"), "HTML element should exist");
        Assert.IsTrue(await ElementExists("head"), "Head element should exist");
        Assert.IsTrue(await ElementExists("body"), "Body element should exist");
        Assert.IsTrue(await ElementExists("style"), "Style element should exist");
    }

    [TestMethod]
    public async Task EmailLayout_ShouldContainAllRequiredSections()
    {
        var html = await RenderEmailHtml<SalesReportModel>(_testMessage);
        await LoadHtml(html);

        Assert.IsTrue(await ElementExists(".email-root"), "Email root container should exist");
        Assert.IsTrue(await ElementExists(".email-container"), "Email container should exist");
        Assert.IsTrue(await ElementExists(".brand-bar"), "Brand bar should exist");
        Assert.IsTrue(await ElementExists(".email-header"), "Email header should exist");
        Assert.IsTrue(await ElementExists(".email-body"), "Email body should exist");
        Assert.IsTrue(await ElementExists(".email-footer"), "Email footer should exist");
    }

    [TestMethod]
    public async Task EmailLayout_ShouldDisplayLogo_InHeader_AndFooter()
    {
        var html = await RenderEmailHtml<SalesReportModel>(_testMessage);
        await LoadHtml(html);

        Assert.IsTrue(await ElementExists(".email-header svg"), "Header logo should exist");
        Assert.IsTrue(await ElementExists(".footer-logo svg"), "Footer logo should exist");
    }

    [TestMethod]
    public async Task EmailContent_ShouldDisplayReportDetails()
    {
        var html = await RenderEmailHtml<SalesReportModel>(_testMessage);
        await LoadHtml(html);

        Assert.IsTrue(await ElementExists(".info-card"), "Info card should exist");
        Assert.IsTrue(await ElementExists(".info-card-title"), "Info card title should exist");

        var cardTitle = await GetElementText(".info-card-title");
        Assert.IsNotNull(cardTitle);
        StringAssert.Contains(cardTitle, "Report Details");
    }

    [TestMethod]
    public async Task EmailContent_ShouldDisplayClientName()
    {
        var html = await RenderEmailHtml<SalesReportModel>(_testMessage);
        await LoadHtml(html);

        var bodyText = await GetElementText(".email-body");
        Assert.IsNotNull(bodyText);
        StringAssert.Contains(bodyText, _testModel.ClientName);
    }

    [TestMethod]
    public async Task EmailContent_ShouldDisplayReportTitle()
    {
        var html = await RenderEmailHtml<SalesReportModel>(_testMessage);
        await LoadHtml(html);

        var bodyText = await GetElementText(".email-body");
        Assert.IsNotNull(bodyText);
        StringAssert.Contains(bodyText, _testModel.Title);
    }

    [TestMethod]
    public async Task EmailContent_ShouldDisplayContactEmailLink()
    {
        var html = await RenderEmailHtml<SalesReportModel>(_testMessage);
        await LoadHtml(html);

        var link = Page.Locator($"a[href='mailto:{_testModel.ContactEmailAddress}']");
        Assert.IsTrue(await link.CountAsync() > 0, "Contact email link should exist");
        
        var linkText = await link.TextContentAsync();
        Assert.AreEqual(_testModel.ContactEmailAddress, linkText, "Link text should match email address");
    }

    [TestMethod]
    public async Task EmailFooter_ShouldDisplayCompanyInfo()
    {
        var html = await RenderEmailHtml<SalesReportModel>(_testMessage);
        await LoadHtml(html);

        Assert.IsTrue(await ElementExists(".footer-address"), "Footer address should exist");
        var footerText = await GetElementText(".email-footer");

        Assert.IsNotNull(footerText);
        StringAssert.Contains(footerText, "Uniphar");
    }
}
