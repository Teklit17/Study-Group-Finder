using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SG_Finder.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public int Age { get; set; }

        // Navigation property to UserProfile
        public virtual UserProfile UserProfile { get; set; }
    }
}