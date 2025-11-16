using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventHub.Config;
using EventHub.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventHub.Services;

public class RabbitMqListener(
    ILogger<RabbitMqListener> logger,
    MessageDispatcher dispatcher,
    IOptions<RabbitMqConfig> options)
    : BackgroundService
{
    private readonly RabbitMqConfig _config = options.Value;
    
    private async Task<IConnection> TryConnectAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config.HostName,
            UserName = _config.UserName,
            Password = _config.Password
        };

        for (var i = 0; i < 10; i++)
        {
            try
            {
                logger.LogInformation("Attempting to connect to RabbitMQ (try {Try})...", i + 1);
                return await factory.CreateConnectionAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "RabbitMQ not ready, retrying in 3 seconds...");
                await Task.Delay(3000, stoppingToken);
            }
        }

        throw new Exception("Failed to connect to RabbitMQ after multiple attempts.");
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting RabbitMQ listener...");

        var connection = await TryConnectAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _config.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var text = Encoding.UTF8.GetString(ea.Body.ToArray());
            logger.LogInformation("Received: {Text}", text);

            await dispatcher.DispatchAsync(
                new Message { Text = text, Timestamp = DateTime.UtcNow });
        };

        await channel.BasicConsumeAsync(
            queue: _config.QueueName,
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken);

        // Keep the background service running
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            // Normal on shutdown
        }
        
        await channel.CloseAsync(cancellationToken: stoppingToken);
        await connection.CloseAsync(cancellationToken: stoppingToken);
    }
}