// Models/UserProfile.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json; // Make sure Newtonsoft.Json is installed via NuGet

namespace SG_Finder.Models
{
    public class UserProfile
    {
        [Key]
        public int UserProfileId { get; set; }  // Primary key

        // Foreign key to ApplicationUser
        [Required]
        public string UserId { get; set; }

        // Navigation property to ApplicationUser
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // Add the input properties for binding form data
        [NotMapped]
        public string SubjectsInput { get; set; } = string.Empty;

        [NotMapped]
        public string AvailabilityInput { get; set; } = string.Empty;

        // Serialized properties for Subjects
        [NotMapped]
        public List<string> Subjects { get; set; } = new List<string>();

        public string SubjectsJson
        {
            get => JsonConvert.SerializeObject(Subjects);
            set => Subjects = string.IsNullOrEmpty(value)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(value);
        }

        // Serialized properties for Availability
        [NotMapped]
        public List<string> Availability { get; set; } = new List<string>();

        public string AvailabilityJson
        {
            get => JsonConvert.SerializeObject(Availability);
            set => Availability = string.IsNullOrEmpty(value)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(value);
        }

        // Other properties...
        [Required]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Bio cannot be longer than 500 characters.")]
        public string Bio { get; set; } = string.Empty;

        public string ProfilePictureUrl { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Study goals cannot be longer than 100 characters.")]
        public string StudyGoals { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Study habits cannot be longer than 100 characters.")]
        public string StudyHabits { get; set; } = string.Empty;
    }
}