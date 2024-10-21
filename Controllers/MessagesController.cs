using Microsoft.AspNetCore.Mvc;
using SG_Finder.Data;
using SG_Finder.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SG_Finder.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

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
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _context.Messages
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();
            return View("~/Views/Messages/Messages.cshtml", messages);
        }
    }
}