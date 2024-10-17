using Microsoft.AspNetCore.Mvc;
using SG_Finder.Data;
using SG_Finder.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SG_Finder.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Notification - List all notifications for a specific user
        public async Task<IActionResult> Index()
        {
            var notifications = await _context.Notifications
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
            return View(notifications);
        }

        // POST: Create a new notification
        [HttpPost]
        public async Task<IActionResult> Create(int userId, string type, string content)
        {
            var notification = new Notification
            {
                UserID = userId,
                Type = type,
                Content = content,
                IsRead = false,
                CreatedDate = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { userId });
        }

        // POST: Mark a notification as read
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Update(notification);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { userId = notification.UserID });
        }
    }
}