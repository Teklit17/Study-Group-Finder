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

        // Action to display all messages (and mark unread messages as read)
        public async Task<IActionResult> Index()
        {
            int userId = 1; // Replace with the authenticated user ID when authentication is implemented

            // Fetch all messages for the current user (the receiver)
            var messages = await _context.Messages
                .Where(m => m.ReceiverID == userId)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            // Mark unread messages as read after they have been viewed
            var unreadMessages = messages.Where(m => !m.IsRead).ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.IsRead = true; // Mark each message as read
                }
                await _context.SaveChangesAsync(); // Save changes to the database
            }

            // Render the Messages.cshtml view with the list of messages
            return View("~/Views/Messages/Messages.cshtml", messages);
        }

        // Optionally, an API to get unread message count for a specific user
        public async Task<int> GetUnreadMessageCount(int userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverID == userId && !m.IsRead)
                .CountAsync();
        }
    }
}
