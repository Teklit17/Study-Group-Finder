// Models/UserProfile.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SG_Finder.Models
{
    public class UserProfile
    {
        [Key]
        public int UserProfileId { get; set; }  // Primary key

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [NotMapped]
        public string SubjectsInput { get; set; } = string.Empty;

        [NotMapped]
        public string AvailabilityInput { get; set; } = string.Empty;

        public string SubjectsJson { get; set; } = string.Empty;

        [NotMapped]
        public List<string> Subjects
        {
            get
            {
                return string.IsNullOrEmpty(SubjectsJson)
                    ? new List<string>()
                    : JsonConvert.DeserializeObject<List<string>>(SubjectsJson);
            }
            set
            {
                SubjectsJson = JsonConvert.SerializeObject(value);
            }
        }

        public string AvailabilityJson { get; set; } = string.Empty;

        [NotMapped]
        public List<string> Availability
        {
            get
            {
                return string.IsNullOrEmpty(AvailabilityJson)
                    ? new List<string>()
                    : JsonConvert.DeserializeObject<List<string>>(AvailabilityJson);
            }
            set
            {
                AvailabilityJson = JsonConvert.SerializeObject(value);
            }
        }

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
