using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CodeArena.Web.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<LeaderboardHub> _hubContext;

    public NotificationService(IHubContext<LeaderboardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageAsync(string messageName, object payload)
        => await _hubContext.Clients.All.SendAsync(messageName, payload);
}
