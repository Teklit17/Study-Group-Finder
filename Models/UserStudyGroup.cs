using System.ComponentModel.DataAnnotations.Schema;

namespace SG_Finder.Models;

public class UserStudyGroup
{
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public int StudyGroupId { get; set; }
    public StudyGroup StudyGroup { get; set; }
}