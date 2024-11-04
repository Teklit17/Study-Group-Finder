using Microsoft.AspNetCore.Mvc;
using SG_Finder.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SG_Finder.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;

namespace SG_Finder.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // Display the profile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(); // User not logged in
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (userProfile == null)
            {
                // Redirect to create profile if not found
                return RedirectToAction("EditProfile");
            }

            // Return the profile view with the user profile data
            return View("~/Views/Profile/Profile.cshtml", userProfile);
        }

        // Display the edit profile form
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (userProfile == null)
            {
                // Create a new profile if one doesn't exist
                userProfile = new UserProfile
                {
                    UserId = user.Id,
                    Username = user.UserName
                };
            }

            return View("~/Views/Profile/_EditProfile.cshtml", userProfile);
        }

        // Save the profile changes
        [HttpPost]
        public async Task<IActionResult> SaveProfile(UserProfile model, IFormFile profilePicture)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == user.Id);

                if (userProfile == null)
                {
                    userProfile = new UserProfile
                    {
                        UserId = user.Id
                    };
                    _context.UserProfiles.Add(userProfile);
                }

                // Update the profile details with the values from the form
                userProfile.Username = model.Username;
                userProfile.Bio = model.Bio;
                userProfile.StudyGoals = model.StudyGoals;
                userProfile.StudyHabits = model.StudyHabits;

                // Process Subjects and Availability from input strings
                userProfile.Subjects = model.SubjectsInput?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList() ?? new List<string>();
                userProfile.Availability = model.AvailabilityInput?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList() ?? new List<string>();

                // Handle profile picture upload if a file is provided
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profilePicture.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(fileStream);
                    }

                    userProfile.ProfilePictureUrl = "/images/" + uniqueFileName;
                }

                await _context.SaveChangesAsync();

                // Redirect to the Index action to display the updated profile
                return RedirectToAction("Index");
            }

            // If the model state is invalid, re-display the form with the current data
            return View("~/Views/Profile/_EditProfile.cshtml", model);
        }

        // Delete the profile
        public async Task<IActionResult> DeleteProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (userProfile != null)
            {
                _context.UserProfiles.Remove(userProfile);
                await _context.SaveChangesAsync();
            }

            // Redirect to a suitable page after deletion, e.g., home page
            return RedirectToAction("Index", "Home");
        }
    }
}
