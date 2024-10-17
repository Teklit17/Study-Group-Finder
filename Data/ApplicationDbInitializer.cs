using System;
using System.Linq;
using SG_Finder.Data; // Replace with your actual namespace for the DbContext
using SG_Finder.Models; // Replace with your actual namespace for the models

public static class ApplicationDbInitializer
{
    public static void Initialize(ApplicationDbContext db)
    {
        // Delete the database before we initialize it. This is common to do during development.
//        db.Database.EnsureDeleted();

        // Recreate the database and tables according to our models
  //      db.Database.EnsureCreated();

        // Add test data to simplify debugging and testing
        var notifications = new[]
        {
            new Notification 
            { 
                UserID = 1, 
                Type = "New Message", 
                Content = "You have received a new message from a study group member!", 
                IsRead = false, 
                CreatedDate = DateTime.Now.AddDays(-2) 
            },
            new Notification 
            { 
                UserID = 1, 
                Type = "Group Invite", 
                Content = "You have been invited to join the 'Math Study Group'.", 
                IsRead = false, 
                CreatedDate = DateTime.Now.AddDays(-1) 
            },
            new Notification 
            { 
                UserID = 2, 
                Type = "Event Reminder", 
                Content = "Reminder: Study session scheduled tomorrow at 5 PM.", 
                IsRead = true, 
                CreatedDate = DateTime.Now 
            }
        };

        db.Notifications.AddRange(notifications);

        // Add test data for messages if none exists
        if (!db.Messages.Any())
        {
            var messages = new[]
            {
                new Message 
                { 
                    SenderID = 1, 
                    ReceiverID = 2, 
                    Content = "Hello! How are you?", 
                    IsRead = false, 
                    SentDate = DateTime.Now.AddMinutes(-10) 
                },
                new Message 
                { 
                    SenderID = 2, 
                    ReceiverID = 1, 
                    Content = "I'm doing well, thanks! How about you?", 
                    IsRead = true, 
                    SentDate = DateTime.Now.AddMinutes(-5) 
                },
                new Message 
                { 
                    SenderID = 1, 
                    ReceiverID = 2, 
                    Content = "Let's study together next week.", 
                    IsRead = false, 
                    SentDate = DateTime.Now.AddMinutes(-1) 
                }
            };

            db.Messages.AddRange(messages);
        }

        // Save all changes to the database
        db.SaveChanges(); // Finally save changes
    }
}
