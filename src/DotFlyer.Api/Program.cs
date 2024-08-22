global using Azure.Identity;
global using Azure.Messaging.ServiceBus;
global using DotFlyer.Api.Extensions;
global using DotFlyer.Api.TopicSenders;
global using DotFlyer.Api.Validators;
global using DotFlyer.Common.Payload;
global using FluentValidation;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Azure;
global using Microsoft.Identity.Web;
global using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
global using System.Text;
global using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddDotFlyerConfiguration();
builder.Services.AddDependencies(builder.Configuration);

builder.Services.AddFluentValidators();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllOrSMS", policy => policy.RequireRole("dotflyer.sender.all", "dotflyer.sender.sms"));
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/dotflyer/healthz/live");

app.UseSwagger(c => { c.RouteTemplate = "dotflyer/open-api/{documentName}"; });

var dotFlyerRouteGroup = app.MapGroup("/dotflyer")
    .AddFluentValidationAutoValidation();

dotFlyerRouteGroup.MapPost("/sms", async ([FromBody] SMSMessage smsMessage, SmsTopicSender smsTopicSender, CancellationToken cancellationToken) =>
    await smsTopicSender.SendMessageAsync(smsMessage, cancellationToken))
    .WithName("PostSMS")
    .WithOpenApi()
    .RequireAuthorization("AllOrSMS");

app.Run();
