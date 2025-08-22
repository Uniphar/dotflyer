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
global using Twilio;
global using Twilio.Rest.Lookups.V2;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddDotFlyerConfiguration();
builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddFluentValidators();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.AddAuthorizationBuilder()
                .AddPolicy("AllOrSMS", policy => policy.RequireRole("dotflyer.sender.all", "dotflyer.sender.sms"))
                .AddPolicy("AllOrEmail", policy => policy.RequireRole("dotflyer.sender.all", "dotflyer.sender.email"));

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/dotflyer/healthz/live");

app.UseSwagger(c => { c.RouteTemplate = "dotflyer/open-api/{documentName}"; });

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
