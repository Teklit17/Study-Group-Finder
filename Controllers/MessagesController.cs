using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SG_Finder.Data;
using SG_Finder.Hubs;
using SG_Finder.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity; // Required for UserManager

namespace SG_Finder.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<StudyHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(ApplicationDbContext context, IHubContext<StudyHub> hubContext, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        // Action to display all messages for the current authenticated user
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(); // User not logged in
            }

            var messages = await _context.Messages
                .Where(m => m.ReceiverID == user.Id)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            // Mark unread messages as read
            var unreadMessages = messages.Where(m => !m.IsRead).ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }

            return View("~/Views/Messages/Messages.cshtml", messages);
        }

        // Action to show the form to create a new message
        public IActionResult _Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user ID
            var message = new Message { SenderID = userId }; // Set the sender ID
            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
   
        public async Task<IActionResult> _Create(Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                // Notify the receiver using SignalR
                await _hubContext.Clients.User(message.ReceiverID)
                    .SendAsync("ReceiveMessage", message.SenderID, message.Content);

             
            }
            return View(message);
        }

        

        // API to get unread message count for the authenticated user
        public async Task<int> GetUnreadMessageCount()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return 0;
            }

            return await _context.Messages
                .Where(m => m.ReceiverID == user.Id && !m.IsRead)
                .CountAsync();
        }
    }
}
