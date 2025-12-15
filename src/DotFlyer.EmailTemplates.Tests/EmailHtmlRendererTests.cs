using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using DotFlyer.Common.Payload;

namespace DotFlyer.EmailTemplates.Tests
{
    [TestClass]
    public class EmailHtmlRendererTests
    {
        private ServiceProvider _provider = null!;

        [TestInitialize]
        public void Init()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddEmailTemplates();

            _provider = services.BuildServiceProvider();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _provider.Dispose();
        }

        [TestMethod]
        public async Task RenderAsync_ShouldProduce_HtmlContainingModel()
        {
            var renderer = new EmailHtmlRenderer(_provider);

            var message = new EmailMessage
            {
                Subject = "Test",
                Body = "This is an integration test email",
                From = new Contact { Email = "sender@test.local", Name = "Sender" },
                To = [new Contact { Email = "to@test.local", Name = "To" }],
                Tags = new Dictionary<string, string> { ["k"] = "v" }
            };

            var html = await renderer.RenderAsync(message);

            Assert.IsFalse(string.IsNullOrWhiteSpace(html));
            StringAssert.Contains(html, message.Body);
            StringAssert.Contains(html, message.Subject);
            StringAssert.Contains(html, message.From.Email);
        }

        [TestMethod]
        public async Task RenderAsync_WithAttachments_ShouldIncludeAttachmentsSection()
        {
            var renderer = new EmailHtmlRenderer(_provider);

            var message = new EmailMessage
            {
                Subject = "Attachments Test",
                Body = "Body",
                From = new Contact { Email = "a@test.local", Name = "A" },
                To = [new Contact { Email = "b@test.local", Name = "B" }],
                Attachments = ["https://storage/att1.txt", "https://storage/att2.csv"]
            };

            var html = await renderer.RenderAsync(message);

            // attachments should be rendered as list items
            StringAssert.Contains(html, "Attachments:");
            StringAssert.Contains(html, "https://storage/att1.txt");
            StringAssert.Contains(html, "https://storage/att2.csv");
        }

        [TestMethod]
        public async Task RenderAsync_WithTags_ShouldIncludeTagsSection()
        {
            var renderer = new EmailHtmlRenderer(_provider);

            var message = new EmailMessage
            {
                Subject = "Tags Test",
                Body = "Body",
                From = new Contact { Email = "a@test.local", Name = "A" },
                To = [new Contact { Email = "b@test.local", Name = "B" }],
                Tags = new Dictionary<string, string> { ["Environment"] = "Integration", ["Id"] = "123" }
            };

            var html = await renderer.RenderAsync(message);

            StringAssert.Contains(html, "Tags:");
            StringAssert.Contains(html, "Environment: Integration");
            StringAssert.Contains(html, "Id: 123");
        }

        [TestMethod]
        public async Task RenderAsync_WithCc_ShouldIncludeCcAddresses()
        {
            var renderer = new EmailHtmlRenderer(_provider);

            var message = new EmailMessage
            {
                Subject = "Cc Test",
                Body = "Body",
                From = new Contact { Email = "a@test.local", Name = "A" },
                To = [new Contact { Email = "b@test.local", Name = "B" }],
                Cc = [new Contact { Email = "cc1@test.local", Name = "CC1" }]
            };

            var html = await renderer.RenderAsync(message);

            StringAssert.Contains(html, "Cc:");
            StringAssert.Contains(html, "CC1");
            StringAssert.Contains(html, "cc1@test.local");
        }

        [TestMethod]
        public async Task RenderAsync_WithHtmlBody_ShouldPreserveMarkup()
        {
            var renderer = new EmailHtmlRenderer(_provider);

            var message = new EmailMessage
            {
                Subject = "Markup Test",
                Body = "<b>Bold content</b>",
                From = new Contact { Email = "a@test.local", Name = "A" },
                To = [new Contact { Email = "b@test.local", Name = "B" }]
            };

            var html = await renderer.RenderAsync(message);

            // since the template outputs MarkupString, the HTML should be present
            StringAssert.Contains(html, "<b>Bold content</b>");
        }

        [TestMethod]
        public async Task RenderAsync_WithOptionalFieldsNull_DoesNotThrow()
        {
            var renderer = new EmailHtmlRenderer(_provider);

            var message = new EmailMessage
            {
                Subject = "Minimal",
                Body = "Body",
                From = new Contact { Email = "a@test.local", Name = "A" },
                To = [new Contact { Email = "b@test.local", Name = "B" }],
                Cc = null,
                Bcc = null,
                Attachments = null,
                Tags = null
            };

            var html = await renderer.RenderAsync(message);

            Assert.IsFalse(string.IsNullOrWhiteSpace(html));
            // Should at least contain subject and body
            StringAssert.Contains(html, message.Subject);
            StringAssert.Contains(html, message.Body);
        }

        [TestMethod]
        public async Task RenderAsync_WriteHtmlToDisk()
        {
            var renderer = new EmailHtmlRenderer(_provider);

            var message = new EmailMessage
            {
                Subject = "Test Email Template",
                Body = "Test Email" +
                       "<p>This email contains <strong>HTML</strong> content.</p>",
                From = new Contact { Email = "sender@test.local", Name = "Sender Name" },
                To = [
                    new Contact { Email = "to1@test.local", Name = "Primary Recipient" },
                    new Contact { Email = "to2@test.local", Name = "Secondary Recipient" }
                ],
                Cc = [
                    new Contact { Email = "cc1@test.local", Name = "CC One" },
                    new Contact { Email = "cc2@test.local", Name = "CC Two" }
                ],
                Bcc = [
                    new Contact { Email = "bcc1@test.local", Name = "BCC One" }
                ],
                Attachments = [
                    "https://storage.example/attachments/report.pdf",
                    "https://storage.example/attachments/data.csv",
                    "https://storage.example/attachments/image.png"
                ],
                Tags = new Dictionary<string, string>
                {
                    ["Environment"] = "IntegrationTest",
                    ["RunId"] = Guid.NewGuid().ToString(),
                    ["Feature"] = "EmailTemplates"
                }
            };

            var html = await renderer.RenderAsync(message);

            var outDir = Path.Combine(Environment.CurrentDirectory, "TestOutputs");
            Directory.CreateDirectory(outDir);
            var filePath = Path.Combine(outDir, $"email_test.html");
            await File.WriteAllTextAsync(filePath, html ?? string.Empty);

            // ensure file was created
            Assert.IsTrue(File.Exists(filePath), $"Expected output file to exist: {filePath}");
        }
    }
}
