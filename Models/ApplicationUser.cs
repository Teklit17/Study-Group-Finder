using Microsoft.AspNetCore.Identity;

namespace SG_Finder.Models;

public class ApplicationUser : IdentityUser
{
    public required string Name { get; set; }
    public int Age { get; set; }
}