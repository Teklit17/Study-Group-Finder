using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SG_Finder.Data;
using SG_Finder.Models;

public static class ApplicationDbInitializer
{
    public static async Task Initialize(ApplicationDbContext db, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm)
    {
        // Ensure database is created (optional: uncomment if you want to recreate the database each time)
        //await db.Database.EnsureDeletedAsync();
        // await db.Database.EnsureCreatedAsync();

        // 1. Ensure the Admin Role Exists
        if (!await rm.RoleExistsAsync("Admin"))
        {
            var adminRole = new IdentityRole("Admin");
            var roleResult = await rm.CreateAsync(adminRole);
            if (!roleResult.Succeeded)
            {
                throw new Exception("Failed to create Admin role.");
            }
        }

        // 2. Create the Admin User
        var adminUser = await um.FindByEmailAsync("admin@uia.no");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin@uia.no",
                Email = "admin@uia.no",
                EmailConfirmed = true,
                Name = "Admin User"
            };

            var userResult = await um.CreateAsync(adminUser, "Password1.");
            if (userResult.Succeeded)
            {
                await um.AddToRoleAsync(adminUser, "Admin");
                await CreateUserProfile(db, adminUser); // Create profile for admin
            }
            else
            {
                throw new Exception("Failed to create Admin user.");
            }
        }

        // 3. Create a Regular User
        var regularUser = await um.FindByEmailAsync("user@uia.no");
        if (regularUser == null)
        {
            regularUser = new ApplicationUser
            {
                UserName = "user@uia.no",
                Email = "user@uia.no",
                EmailConfirmed = true,
                Name = "Regular User"
            };

            var userResult = await um.CreateAsync(regularUser, "Password1.");
            if (userResult.Succeeded)
            {
                await CreateUserProfile(db, regularUser); // Create profile for regular user
            }
            else
            {
                throw new Exception("Failed to create Regular user.");
            }
        }

        // 4. Add Test Notifications (if none exist)
        if (!db.Notifications.Any())
        {
            var notifications = new[]
            {
                new Notification
                {
                    UserID = regularUser.Id,  // Assign to the regular user
                    Type = "Reminder",
                    Content = "Your study group meeting is scheduled for tomorrow.",
                    IsRead = false,
                    CreatedDate = DateTime.Now.AddDays(-1)
                }
            };

            db.Notifications.AddRange(notifications);
        }

        // 5. Add Test Messages (if none exist)
        if (!db.Messages.Any())
        {
            var newMessage = new Message
            {
                SenderID = adminUser.Id,    // Admin sends the message
                ReceiverID = regularUser.Id, // Regular user receives it
                Content = "Let's study together next week.",
                IsRead = false,
                SentDate = DateTime.Now
            };

            db.Messages.Add(newMessage);
        }

        // 6. Save Changes to the Database
        await db.SaveChangesAsync();
    }

    // Helper Method: Create UserProfile for a User
    private static async Task CreateUserProfile(ApplicationDbContext db, ApplicationUser user)
    {
        // Check if the profile already exists
        var existingProfile = await db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (existingProfile == null)
        {
            // Create a new profile
            var userProfile = new UserProfile
            {
                UserId = user.Id,
                Username = user.UserName, // Default username from the ApplicationUser
                Bio = "",
                StudyGoals = "",
                StudyHabits = ""
            };

            db.UserProfiles.Add(userProfile);
            await db.SaveChangesAsync();
        }
    }
}
