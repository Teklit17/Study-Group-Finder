using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SG_Finder.Models // Adjust the namespace based on your project structure
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }

        [Required]
        public string SenderID { get; set; }

        [ForeignKey("SenderID")]
        public ApplicationUser Sender { get; set; } // Navigation property
        
        
        [Required]
        public string ReceiverID { get; set; }


        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty; // Message content

        [DataType(DataType.DateTime)]
        public DateTime SentDate { get; set; } = DateTime.Now; // Timestamp for when the message was sent

        public bool IsRead { get; set; } = false;  // Read status
    }
}