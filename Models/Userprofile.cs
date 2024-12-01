using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SG_Finder.Models
{
    public class UserProfile
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }  // Use UserId as the primary key

        public string Username { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string StudyGoals { get; set; } = string.Empty;
        public string StudyHabits { get; set; } = string.Empty;

        // Add this property to store the profile picture URL
        public string ProfilePictureUrl { get; set; } = string.Empty;

        // Navigation property to ApplicationUser
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}