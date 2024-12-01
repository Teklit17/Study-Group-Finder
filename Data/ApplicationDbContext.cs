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
        public DbSet<UserProfile> UserProfiles { get; set; }

        // Add Events DbSet
        public DbSet<Event> Events { get; set; }
        
        // DbSet for StudyGroups
        public DbSet<StudyGroup> StudyGroups { get; set; }
        public DbSet<UserStudyGroup> UserStudyGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // UserStudyGroup
            builder.Entity<UserStudyGroup>()
                .HasKey(ug => new { ug.ApplicationUserId, ug.StudyGroupId });

            // Configure one-to-one relationship between ApplicationUser and UserProfile
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.UserProfile)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<UserProfile>(p => p.UserId);
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