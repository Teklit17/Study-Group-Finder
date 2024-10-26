using Microsoft.AspNetCore.Mvc;
using SG_Finder.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SG_Finder.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NotificationBellViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            // Handle null or empty userId
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.UnreadCount = 0; // Set unread count to 0 if userId is null
                return View("/Views/Notification/NotificationBell.cshtml");
            }

            // Proceed with the query only if userId is valid
            var unreadCount = await _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .CountAsync();

            ViewBag.UnreadCount = unreadCount;

            // Return the view with notification bell
            return View("/Views/Notification/NotificationBell.cshtml");
        }


    }
}