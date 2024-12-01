using Microsoft.AspNetCore.Mvc;
using SG_Finder.Data;
using SG_Finder.Models; // Ensure you have access to your Message and MessageBellViewModel models
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
            if (string.IsNullOrEmpty(userId))
            {
                // Log or handle if userId is missing
                return View(new MessageBellViewModel
                {
                    UnreadMessageCount = 0,
                    UnreadMessages = new List<Message>() // Empty list if userId is invalid
                });
            }

            // Fetch unread messages along with the sender's details
            var unreadMessages = await _context.Messages
                .Where(m => m.ReceiverID == userId && !m.IsRead)
                .Include(m => m.Sender) // Include Sender to access their email or name
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            // Prepare the view model
            var viewModel = new MessageBellViewModel
            {
                UnreadMessageCount = unreadMessages.Count,
                UnreadMessages = unreadMessages
            };

            // Return the view with the model
            return View("/Views/Messages/MessageBell.cshtml", viewModel);
        }
    }
}