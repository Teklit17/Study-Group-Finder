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
                .Where(m => m.ReceiverID == userId && !m.IsRead)  // Correct filtering
                .CountAsync();

        
            // Log the unread message count to verify
            Console.WriteLine($"Unread messages for user {userId}: {unreadMessageCount}");
         
            ViewBag.UnreadMessageCount = unreadMessageCount;

            // Specify the custom path to the view
            return View("/Views/Messages/MessageBell.cshtml");
        }
    }
}