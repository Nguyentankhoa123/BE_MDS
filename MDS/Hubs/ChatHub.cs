using Microsoft.AspNetCore.SignalR;

namespace MDS.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessageToGroup(string groupName, string userId, string user, string message, string nameChat)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", userId, user, message, nameChat);
        }

    }



}
