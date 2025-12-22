using DotFlyer.Common.Payload;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace DotFlyer.EmailTemplates.Tests;

[TestClass]
public abstract class PlaywrightTestBase
{
    protected ServiceProvider Provider = null!;
    protected IPlaywright PlaywrightInstance = null!;
    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;
    protected string OutputDirectory = null!;

    [TestInitialize]
    public async Task PlaywrightSetup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddEmailTemplates();
        Provider = services.BuildServiceProvider();

        OutputDirectory = Path.Combine(Environment.CurrentDirectory, "PlaywrightTestOutputs");
        Directory.CreateDirectory(OutputDirectory);

        PlaywrightInstance = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Channel = "msedge",
            Headless = true
        });
        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
        });
        Page = await Context.NewPageAsync();
    }

    [TestCleanup]
    public async Task PlaywrightCleanup()
    {
        await Page?.CloseAsync()!;
        await Context?.CloseAsync()!;
        await Browser?.CloseAsync()!;
        PlaywrightInstance?.Dispose();
        Provider?.Dispose();
    }

    protected async Task<string> RenderEmailHtml<T>(EmailMessage message) where T : class
    {
        var renderer = new EmailHtmlRenderer(Provider);
        var html = await renderer.RenderAsync(message);
        
        var fileName = $"{typeof(T).Name}_{DateTime.Now:yyyyMMddHHmmss}.html";
        var filePath = Path.Combine(OutputDirectory, fileName);
        await File.WriteAllTextAsync(filePath, html);
        return html;
    }

    protected async Task LoadHtml(string html)
    {
        await Page.SetContentAsync(html);
    }

    protected async Task<bool> ElementExists(string selector)
    {
        return await Page.Locator(selector).CountAsync() > 0;
    }

    protected async Task<string?> GetElementText(string selector)
    {
        var element = Page.Locator(selector);
        if (await element.CountAsync() > 0)
        {
            return await element.TextContentAsync();
        }
        return null;
    }

    protected async Task TakeScreenshot(string name)
    {
        var screenshotPath = Path.Combine(OutputDirectory, $"{name}_{DateTime.Now:yyyyMMddHHmmss}.png");
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = screenshotPath,
            FullPage = true
        });
    }
}
