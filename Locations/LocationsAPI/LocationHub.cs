using Microsoft.AspNetCore.SignalR;

namespace LocationsAPI
{
    public class LocationHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReciveMessage", message);
        }
    }
}
