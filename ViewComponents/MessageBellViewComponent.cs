using Microsoft.AspNetCore.Mvc;
using SG_Finder.Data;
using SG_Finder.Models; // Ensure you have access to your Message model
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SG_Finder.ViewComponents
{
    public class MessageBellViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public MessageBellViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            // Fetch unread messages and count
            var unreadMessages = await _context.Messages
                .Where(m => m.ReceiverID == userId && !m.IsRead)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            var viewModel = new MessageBellViewModel
            {
                UnreadMessageCount = unreadMessages.Count,
                UnreadMessages = unreadMessages
            };

            Console.WriteLine($"Unread messages for user {userId}: {viewModel.UnreadMessageCount}");

            // Return the view with the model
            return View("/Views/Messages/MessageBell.cshtml", viewModel);
        }
    }
}