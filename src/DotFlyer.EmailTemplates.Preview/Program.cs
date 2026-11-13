using DotFlyer.EmailTemplates;
using DotFlyer.EmailTemplates.Preview;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(TemplateCatalog.Discover(typeof(EmailHtmlRenderer).Assembly));
builder.Services.AddRazorPages();

var app = builder.Build();
app.MapPreviewEndpoints();
app.MapRazorPages();
app.Run();
