using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebAPI.Services
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinRoom(string user, string RoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, RoomId);
            await Clients.Group(RoomId).SendAsync("ReceiveMessage", user, "message");
        }

        public async Task SendUserMessage(string userId, string RoomId)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage", "message");
        }
    }
}
