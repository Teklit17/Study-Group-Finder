using Microsoft.AspNetCore.Identity;

namespace SG_Finder.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }

        // Direct relationship to UserProfile
        public virtual UserProfile UserProfile { get; set; }

        // User and StudyGroup many-to-many relationship
        public ICollection<UserStudyGroup> StudyGroups { get; set; } = new List<UserStudyGroup>();
    }
}