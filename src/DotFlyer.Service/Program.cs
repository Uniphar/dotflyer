global using DotFlyer.Service;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<MessageProcessor>();

var host = builder.Build();
host.Run();
