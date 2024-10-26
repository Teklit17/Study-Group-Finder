using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SG_Finder.Data;
using SG_Finder.Models;
using System.Threading.Tasks;

namespace SG_Finder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Dependency inject the db context, user manager, and role manager
        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Action method to create the Admin role and assign the user to the role
        public async Task<IActionResult> CreateAdminUser()
        {
            // Check if the role exists, if not, create it
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!roleResult.Succeeded)
                {
                    return Content("Failed to create the Admin role.");
                }
            }

            // Check if the admin user exists, if not, create the user and assign to the role
            var adminUser = await _userManager.FindByEmailAsync("admin@uia.no");
            if (adminUser == null)
            {
                // FIX: Ensure the Name property is set for the ApplicationUser
                adminUser = new ApplicationUser
                {
                    UserName = "admin@uia.no",
                    Email = "admin@uia.no",
                    Name = "Admin User" // Add Name property initialization
                };
        
                var userResult = await _userManager.CreateAsync(adminUser, "Password1.");
                if (userResult.Succeeded)
                {
                    // Assign the user to the "Admin" role
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    return Content("Admin user created and assigned to the Admin role.");
                }
                else
                {
                    return Content("Failed to create the admin user.");
                }
            }
            else
            {
                // If the user already exists, ensure they are assigned to the "Admin" role
                if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
                return Content("Admin user already exists and is assigned to the Admin role.");
            }
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Messages()
        {
            return RedirectToAction("Index", "Messages");
        }

        public IActionResult Notification()
        {
            return RedirectToAction("Index", "Notification");
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

    
        public IActionResult StudyGroupFinder()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
