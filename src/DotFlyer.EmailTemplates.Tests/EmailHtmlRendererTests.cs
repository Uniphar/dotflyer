using DotFlyer.Common.Payload;
using DotFlyer.EmailTemplates.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotFlyer.EmailTemplates.Tests
{
    [TestClass]
    public class EmailHtmlRendererTests
    {
        private ServiceProvider _provider = null!;
        private string _outputDirectory = null!;

        [TestInitialize]
        public void Init()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddEmailTemplates();

            _provider = services.BuildServiceProvider();

            _outputDirectory = Path.Combine(Environment.CurrentDirectory, "TestOutputs");
            Directory.CreateDirectory(_outputDirectory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _provider.Dispose();
        }

        [TestMethod]
        public async Task RenderAsync_ManualSecretRotation_ShouldRenderTemplateSuccessfully()
        {
            var renderer = new EmailHtmlRenderer(_provider);

            var model = new ManualSecretRotationModel
            {
                SecretName = "DbPassword",
                TenantId = Guid.NewGuid().ToString(),
                AppId = Guid.NewGuid().ToString(),  
                ResourceName = "sqlserver1",
                KeyVaults = new List<string>
                {
                    "https://kv1.vault.local",
                    "https://kv2.vault.local"
                },
                OldSecretDeletionDateUtc = DateTime.UtcNow.AddDays(7),
                PwPushUrl = "https://pwpush.local/p/abcdefg",
                PwPushExpiresAfterViews = 5,
                PwPushExpiresInDays = 3
            };
            var message = new EmailMessage
            {
                Subject = "Manual Secret Rotation Required",
                Body = "Fallback body",
                TemplateId = nameof(ManualSecretRotationModel),
                TemplateModel = model,
                From = new Contact { Email = "sender@test.local", Name = "Sender" },
                To = [new Contact { Email = "to@test.local", Name = "To" }]
            };

            var html = await renderer.RenderAsync(message);

            Assert.IsFalse(string.IsNullOrWhiteSpace(html));
            StringAssert.Contains(html, model.TenantId);
            StringAssert.Contains(html, model.AppId);
            StringAssert.Contains(html, model.ResourceName);
            StringAssert.Contains(html, model.PwPushUrl);

            await WriteHtmlToFile("ManualSecretRotation.html", html);
        }

        [TestMethod]
        public async Task RenderAsync_SalesReport_ShouldRenderTemplateSuccessfully()
        {
            var renderer = new EmailHtmlRenderer(_provider);

            var model = new SalesReportModel
            {
                Title = "Sales Report - January 2024",
                ClientName = "Acme Corporation",
                ContactEmailAddress = "support@acme.com",
            };
            var message = new EmailMessage
            {
                Subject = "Monthly Sales Report",
                Body = "Fallback body",
                TemplateId = nameof(SalesReportModel),
                TemplateModel = model,
                From = new Contact { Email = "sender@test.local", Name = "Sender" },
                To = [new Contact { Email = "to@test.local", Name = "To" }]
            };

            var html = await renderer.RenderAsync(message);

            Assert.IsFalse(string.IsNullOrWhiteSpace(html));
            StringAssert.Contains(html, model.Title);
            StringAssert.Contains(html, model.ClientName);
            StringAssert.Contains(html, model.ContactEmailAddress);

            await WriteHtmlToFile("SalesReport.html", html);
        }

        [TestMethod]
        public async Task RenderAsync_NoTemplate_ShouldReturnBody()
        {
            var renderer = new EmailHtmlRenderer(_provider);

            var message = new EmailMessage
            {
                Subject = "Test",
                Body = "This is a plain email without template",
                From = new Contact { Email = "sender@test.local", Name = "Sender" },
                To = [new Contact { Email = "to@test.local", Name = "To" }]
            };

            var html = await renderer.RenderAsync(message);

            Assert.IsFalse(string.IsNullOrWhiteSpace(html));
            StringAssert.Contains(html, "This is a plain email without template");
        }

        private async Task WriteHtmlToFile(string fileName, string html)
        {
            var filePath = Path.Combine(_outputDirectory, fileName);
            await File.WriteAllTextAsync(filePath, html ?? string.Empty);
            Console.WriteLine($"HTML output written to: {filePath}");
            Assert.IsTrue(File.Exists(filePath), $"Expected output file to exist: {filePath}");
        }
    }
}
