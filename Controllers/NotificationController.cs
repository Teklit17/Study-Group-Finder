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
            // TEMPORARY: Hardcoded userId for now, replace this with actual userId later (1 is used for now based on your database)
            int userId = 1;

            // Get all notifications for the user
            var notifications = await _context.Notifications
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            // Get unread notification count (IsRead = false)
            ViewBag.UnreadCount = await CountUnreadNotifications(userId);

            return View(notifications);
        }

        // Count the unread notifications for the user
        public async Task<int> CountUnreadNotifications(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .CountAsync();
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
