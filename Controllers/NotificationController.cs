using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SG_Finder.Data;
using SG_Finder.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SG_Finder.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Fetches notifications for the current user
        public async Task<IActionResult> Index()
        {
            // Get the current user's ID
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            // Fetch notifications asynchronously
            var notifications = await _context.Notifications
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return View(notifications);
        }

        // Marks a notification as read
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            // Find the notification by ID
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null || notification.UserID != userId)
            {
                return NotFound();
            }

            // Update notification status
            notification.IsRead = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
