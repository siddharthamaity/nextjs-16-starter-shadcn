using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using EventHub.Services;
using EventHub.Models;
using EventHub.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventHub.Services;

public class RabbitMqListener
{
    private readonly ILogger<RabbitMqListener> _logger;
    private readonly MessageDispatcher _dispatcher;
    private readonly RabbitMqConfig _config;

    public RabbitMqListener(ILogger<RabbitMqListener> logger, MessageDispatcher dispatcher,
        IOptions<RabbitMqConfig> options)
    {
        _logger = logger;
        _dispatcher = dispatcher;
        _config = options.Value;
        _ = StartListeningAsync(); // fire-and-forget
    }

    private async Task StartListeningAsync()
    {
        _logger.LogInformation("[*] RabbitMQ listener starting. Waiting for messages...");

        var factory = new ConnectionFactory
        {
            HostName = _config.HostName,
            UserName = _config.UserName,
            Password = _config.Password
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: _config.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageText = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Received message: {Message}", messageText);

            var message = new Message
            {
                Text = messageText,
                Timestamp = DateTime.UtcNow
            };

            await _dispatcher.DispatchAsync(message);
        };

        await channel.BasicConsumeAsync(
            queue: _config.QueueName,
            autoAck: true,
            consumer: consumer
        );
    }
}
