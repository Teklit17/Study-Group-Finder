using Microsoft.AspNetCore.Mvc;
using SG_Finder.Data;
using SG_Finder.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SG_Finder.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action to create a new message
        public IActionResult _Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create([Bind("SenderID,ReceiverID,Content")] Message message)
        {
            if (ModelState.IsValid)
            {
                message.SentDate = DateTime.Now;
                message.IsRead = false; // Mark the message as unread by default
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        // Action to display all messages (including marking messages as read)
        public async Task<IActionResult> Index()
        {
            int userId = 1; // Replace with authenticated user ID when ready

            // Fetch all messages for the current user
            var messages = await _context.Messages
                .Where(m => m.ReceiverID == userId)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            // Mark unread messages as read once viewed
            var unreadMessages = messages.Where(m => !m.IsRead).ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.IsRead = true; // Mark each message as read
                }
                await _context.SaveChangesAsync(); // Save the changes
            }

            return View("~/Views/Messages/Messages.cshtml", messages);
        }

        // Optionally, you can create an API to return the count of unread messages for a specific user
        public async Task<int> GetUnreadMessageCount(int userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverID == userId && !m.IsRead)
                .CountAsync();
        }
    }
}
