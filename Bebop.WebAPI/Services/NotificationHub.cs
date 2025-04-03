using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI.Services.Extentions;
using WebAPI.Services.Repositories;

namespace WebAPI.Services
{
    [Authorize]
    public class NotificationHub(IUserRepository userRepository) : Hub
    {
        public override Task OnConnectedAsync()
        {
            var user = Context.User.Convert();
            userRepository.Create(user);

            Console.WriteLine("A Client Connected: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            var userMail = Context.User.FindFirst(ClaimTypes.Email)?.Value;
            Console.WriteLine("A client disconnected: " + Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string addressee, string message)
        {
            string addresseeId = null;
            if (IsValidGuid(addressee))
                addresseeId = addressee.ToUpper();
            else
            {
                var userNotification = userRepository.GetByEmail(addressee);
                addresseeId = userNotification.UserId;
            }
            var user = Context.User.Convert();

            await Clients.User(addresseeId).SendAsync("ReceiveMessage", $"{user.Name}: {message}");
        }

        public async Task SendUserMessage(string userId, string RoomId)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage", "message");
        }
        bool IsValidGuid(string input) => Guid.TryParse(input, out _);
    }
}
