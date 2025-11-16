using System.Threading.Tasks;
using EventHub.Models;
using Microsoft.Extensions.Logging;

public class MessageDispatcher
{
    private readonly ILogger<MessageDispatcher> _logger;

    public MessageDispatcher(ILogger<MessageDispatcher> logger)
    {
        _logger = logger;
    }

    public Task DispatchAsync(Message message)
    {
        _logger.LogInformation("Dispatching message: {Message}", message.Text);
        return Task.CompletedTask;
    }
}