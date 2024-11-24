using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SG_Finder.Hubs
{
    
    public class StudyHub : Hub
    {
        public async Task SendMessageToUser(string receiverId, string senderId, string content)
        {
            // Notify the specific recipient
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, content);
        }

        public async Task NotifyNewMessage(string receiverId, string senderId, string content)
        {
            // Notify the user that they have a new message
            await Clients.User(receiverId).SendAsync("NewMessageNotification", senderId, content);
        }
    }


}