using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace WebAPI.Services
{
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("A Client Connected: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("A client disconnected: " + Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
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
