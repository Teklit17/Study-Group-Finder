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

        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            var unreadCount = await _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .CountAsync();

            ViewBag.UnreadCount = unreadCount;

            // Specify the full path to the view
            return View("/Views/Notification/NotificationBell.cshtml");
        }

    }
}