using System.Threading.Tasks;
using EventHub.Models;
using EventHub.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace EventHub.Services;

public class MessageDispatcher
{
    private readonly IHubContext<MessageHub> _hubContext;

    public MessageDispatcher(IHubContext<MessageHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task DispatchAsync(Message message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
    }
}