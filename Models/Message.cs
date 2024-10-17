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
        public int SenderID { get; set; }     // Foreign key to Users table (sender)

        [Required]
        public int ReceiverID { get; set; }   // Foreign key to Users table (receiver)

        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty; // Message content

        [DataType(DataType.DateTime)]
        public DateTime SentDate { get; set; } = DateTime.Now; // Timestamp for when the message was sent

        public bool IsRead { get; set; } = false;  // Read status
    }
}