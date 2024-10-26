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

        // DbSet for Messages - Ensure that Message model has the correct FK definitions
        public DbSet<Message> Messages { get; set; }

        // DbSet for Notifications - Ensure Notification model has the necessary properties
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Additional configurations if needed, e.g., setting up relationships
            // Example: builder.Entity<Message>().HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderID);
        }
    }
}