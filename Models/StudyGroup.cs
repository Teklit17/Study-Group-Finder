using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG_Finder.Models;

public class StudyGroup
{
    public int Id { get; set; }

    [Required] [StringLength(20)] 
    public string GroupName { get; set; } = string.Empty;

    [Required] [StringLength(500)] 
    public string GroupDescription { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 20)]
    public int MaxGroupMembers { get; set; }
    
    public string CreatorId { get; set; } = string.Empty;
    
    public ICollection<UserStudyGroup> GroupMembers { get; set; } = new List<UserStudyGroup>();
}