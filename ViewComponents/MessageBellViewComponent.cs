using Microsoft.AspNetCore.Mvc;
using SG_Finder.Data;
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

        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            // Fetch messages for the receiver (the current user) that are unread
            var unreadMessageCount = await _context.Messages
                .Where(m => m.ReceiverID == userId && !m.IsRead)
                .CountAsync();

            // Store the unread message count in the ViewBag for use in the view
            ViewBag.UnreadMessageCount = unreadMessageCount;

            // Log the unread message count for debugging purposes
            Console.WriteLine($"Unread messages for user {userId}: {unreadMessageCount}");

            // Return the view for the message bell (adjust the view path if necessary)
            return View("/Views/Messages/MessageBell.cshtml");
        }
    }
}