using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SG_Finder.Models;
using SG_Finder.Data;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SG_Finder.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // View the profile
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            if (userId == null)
                return Unauthorized(); // User not logged in

            var profile = await _context.UserProfiles.FindAsync(userId);

            if (profile == null)
                return RedirectToAction(nameof(EditProfile)); // Redirect to create/edit profile if it doesn't exist

            return View("~/Views/Profile/Index.cshtml", profile);
        }

        // Display the form to create/edit a profile
        // Controllers/ProfileController.cs
        public async Task<IActionResult> EditProfile()
        {
            // Fetch the current user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(); // Return if the user is not logged in
            }

            // Check if the user's profile exists
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (userProfile == null)
            {
                // Only create a new profile if one does not exist
                userProfile = new UserProfile
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Bio = "", // Optional: initialize other fields with default values
                    StudyGoals = "",
                    StudyHabits = ""
                };

                // Add the new profile to the database
                _context.UserProfiles.Add(userProfile);

                try
                {
                    // Save changes to persist the new profile
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Error saving UserProfile: {ex.Message}");
                    return StatusCode(500, "An error occurred while creating the user profile.");
                }
            }

            // Return the view with the user profile
            return View("~/Views/Profile/EditProfile.cshtml", userProfile);
        }



        // Save the profile changes
        [HttpPost]
        public async Task<IActionResult> SaveProfile(UserProfile model)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var existingProfile = await _context.UserProfiles.FindAsync(userId);

            if (existingProfile == null)
            {
                // Create a new profile if none exists
                model.UserId = userId;
                _context.UserProfiles.Add(model);
            }
            else
            {
                // Update the existing profile
                existingProfile.Username = model.Username;
                existingProfile.Bio = model.Bio;
                existingProfile.StudyGoals = model.StudyGoals;
                existingProfile.StudyHabits = model.StudyHabits;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // Redirect back to the profile view
        }

        // Delete the profile
        [HttpPost]
        public async Task<IActionResult> DeleteProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var profile = await _context.UserProfiles.FindAsync(userId);

            if (profile != null)
            {
                _context.UserProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
