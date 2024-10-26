namespace SG_Finder.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;



public class Notification
{
    
    public Notification() {}

    public int NotificationID { get; set; }

    [Required]
    public string UserID { get; set; }  // Foreign key to Users table

    [Required]
    [StringLength(50)]
    [DisplayName("Notification Type")]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [DisplayName("Content")]
    public string Content { get; set; } = string.Empty;

    [DisplayName("Read Status")]
    public bool IsRead { get; set; } = false;

    [DataType(DataType.DateTime)]
    [DisplayName("Created Date")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
