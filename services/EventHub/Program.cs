using EventHub.Hubs;
using EventHub.Services;
using EventHub.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Load config
builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMQ"));

// Add SignalR
builder.Services.AddSignalR();

// Add RabbitMqListener & MessageDispatcher as singletons
builder.Services.AddSingleton<MessageDispatcher>();
builder.Services.AddHostedService<RabbitMqListener>();

var app = builder.Build();

app.MapHub<MessageHub>("/messages");

app.Run();