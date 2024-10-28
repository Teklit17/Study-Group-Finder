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
        
        // DbSet for StudyGroups
        public DbSet<StudyGroup> StudyGroups { get; set; }
        public DbSet<UserStudyGroup> UserStudyGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // UserStudyGroup
            builder.Entity<UserStudyGroup>()
                .HasKey(ug => new { ug.ApplicationUserId, ug.StudyGroupId });

            // Relationship between ApplicationUser and UserStudyGroup
            builder.Entity<UserStudyGroup>()
                .HasOne(ug => ug.ApplicationUser)
                .WithMany(u => u.StudyGroups)
                .HasForeignKey(ug => ug.ApplicationUserId);

            // Relationship between StudyGroup and UserStudyGroup
            builder.Entity<UserStudyGroup>()
                .HasOne(ug => ug.StudyGroup)
                .WithMany(sg => sg.GroupMembers)
                .HasForeignKey(ug => ug.StudyGroupId);

            // Additional configurations if needed, e.g., setting up relationships
            // Example: builder.Entity<Message>().HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderID);
        }
    }
}