using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SG_Finder.Hubs
{
    public class StudyHub : Hub
    {
        // Method to send a message to a specific user
        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier; // Get the current connected user's ID
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);
        }
    }


}