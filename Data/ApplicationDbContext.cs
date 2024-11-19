using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SG_Finder.Models;

namespace SG_Finder.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Existing DbSets
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // Add UserProfiles DbSet
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationship between ApplicationUser and UserProfile
            builder.Entity<UserProfile>()
                .HasOne(up => up.User)
                .WithOne(u => u.UserProfile)
                .HasForeignKey<UserProfile>(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            
            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderID)
                .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}