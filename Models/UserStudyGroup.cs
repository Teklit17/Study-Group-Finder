using System.ComponentModel.DataAnnotations.Schema;

namespace SG_Finder.Models;

public class UserStudyGroup
{
    public required string ApplicationUserId { get; set; }
    public required ApplicationUser ApplicationUser { get; set; }

    public int StudyGroupId { get; set; }
    public required StudyGroup StudyGroup { get; set; }
    
}