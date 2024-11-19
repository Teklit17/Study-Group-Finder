using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SG_Finder.Data;
using SG_Finder.Hubs;
using SG_Finder.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

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

        // GET: /Messages/Contacts
        [HttpGet]
        public async Task<IActionResult> Contacts()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null) return Unauthorized();

            var users = await _context.Users
                .Where(u => u.Id != currentUserId)
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            return View(users);
        }

        // GET: /Messages/Chat/{receiverId}
        public async Task<IActionResult> Chat(string receiverId)
        {
            ViewBag.ReceiverID = receiverId;
            var currentUserId = _userManager.GetUserId(User);

            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Where(m => (m.SenderID == currentUserId && m.ReceiverID == receiverId) ||
                            (m.SenderID == receiverId && m.ReceiverID == currentUserId))
                .OrderBy(m => m.SentDate)
                .ToListAsync();

            return View(messages);
        }



        // POST: /Messages/SendMessage
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.ReceiverID) || string.IsNullOrEmpty(message.Content))
            {
                return BadRequest("Invalid message data.");
            }

            var senderId = _userManager.GetUserId(User);
            if (senderId == null)
            {
                return Unauthorized();
            }

            message.SenderID = senderId;
            message.SentDate = DateTime.Now;
            message.IsRead = false;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok();
        }
        public async Task<IActionResult> GetUnreadMessages()
        {
            var currentUserId = _userManager.GetUserId(User);

            var unreadMessages = await _context.Messages
                .Where(m => m.ReceiverID == currentUserId && !m.IsRead)
                .Include(m => m.Sender) // Include the Sender to access their email
                .ToListAsync();

            var viewModel = new MessageBellViewModel
            {
                UnreadMessages = unreadMessages,
                UnreadMessageCount = unreadMessages.Count
            };

            return PartialView("_MessageDropdown", viewModel);
        }


    }
}
