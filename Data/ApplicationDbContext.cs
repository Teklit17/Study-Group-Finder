using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Required for IdentityDbContext
using SG_Finder.Models;

namespace SG_Finder.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Notification> Notifications { get; set; }
    }
}