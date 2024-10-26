using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SG_Finder.Data; // Ensure this matches your actual namespace
using SG_Finder.Models; // Ensure this matches your actual namespace

public static class ApplicationDbInitializer
{
    public static async Task Initialize(ApplicationDbContext db, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm)
    {
        // Delete and recreate the database to ensure a fresh start
      //  await db.Database.EnsureDeletedAsync();
      //  await db.Database.EnsureCreatedAsync();

        // Create the Admin role if it doesn't exist
        if (!await rm.RoleExistsAsync("Admin"))
        {
            var adminRole = new IdentityRole("Admin");
            var roleResult = await rm.CreateAsync(adminRole);
            if (!roleResult.Succeeded)
            {
                throw new Exception("Failed to create Admin role.");
            }
        }

        // Ensure the Admin user exists, and if not, create one
        var adminUser = await um.FindByEmailAsync("admin@uia.no");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin@uia.no",
                Email = "admin@uia.no",
                EmailConfirmed = true,
                Name = "Admin User" // Setting the Name field
            };

            var userResult = await um.CreateAsync(adminUser, "Password1.");
            if (userResult.Succeeded)
            {
                await um.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                throw new Exception("Failed to create Admin user.");
            }
        }

        // Ensure a regular user exists, and if not, create one
        var regularUser = await um.FindByEmailAsync("user@uia.no");
        if (regularUser == null)
        {
            regularUser = new ApplicationUser
            {
                UserName = "user@uia.no",
                Email = "user@uia.no",
                EmailConfirmed = true,
                Name = "Regular User" // Setting the Name field
            };

            var userResult = await um.CreateAsync(regularUser, "Password1.");
            if (!userResult.Succeeded)
            {
                throw new Exception("Failed to create regular user.");
            }
        }

        // Add test notifications for the Admin and regular users if none exist
        if (!db.Notifications.Any())
        {
            var notifications = new[]
            {
                new Notification
                {
                    UserID = adminUser.Id,  
                    Type = "New Message",
                    Content = "You have received a new message from a study group member!",
                    IsRead = false,
                    CreatedDate = DateTime.Now.AddDays(-2)
                },
                new Notification
                {
                    UserID = regularUser.Id,  
                    Type = "Reminder",
                    Content = "Your study group meeting is scheduled for tomorrow.",
                    IsRead = false,
                    CreatedDate = DateTime.Now.AddDays(-1)
                }
            };

            db.Notifications.AddRange(notifications);
        }

        // Add a test message from the Admin user to the regular user if none exist
        if (!db.Messages.Any())
        {
            var newMessage = new Message
            {
                SenderID = adminUser.Id,   
                ReceiverID = regularUser.Id, 
                Content = "Let's study together next week.",
                IsRead = false,
                SentDate = DateTime.Now
            };

            db.Messages.Add(newMessage); // Add the new message to the database
        }

        // Save all changes
        await db.SaveChangesAsync();
    }
}
