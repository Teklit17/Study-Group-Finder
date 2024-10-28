using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SG_Finder.Data;
using SG_Finder.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SG_Finder.Controllers;

[Authorize]
public class StudyGroupController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudyGroupController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var studyGroups = _context.StudyGroups
            .Include(sg => sg.GroupMembers)
            .ThenInclude(ug => ug.ApplicationUser)
            .ToList();

        return View(studyGroups);
    }

    // Create
    // public IActionResult Create()
    // {
    //     return View();
    // }
    
    [HttpPost]
    public async Task<IActionResult> Create(StudyGroup studyGroup)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            studyGroup.CreatorId = userId;
            
            var creator = await _userManager.FindByIdAsync(userId);
            if (creator == null)
            {
                return Unauthorized();
            }

            var userStudyGroup = new UserStudyGroup
            {
                StudyGroup = studyGroup,
                ApplicationUserId = userId,
                ApplicationUser = creator
            };

            studyGroup.GroupMembers.Add(userStudyGroup);

            _context.StudyGroups.Add(studyGroup);
            await _context.SaveChangesAsync();
        
            return Json(new
            {
                id = studyGroup.Id, 
                groupName = studyGroup.GroupName, 
                groupDescription = studyGroup.GroupDescription, 
                maxGroupMembers = studyGroup.MaxGroupMembers,
                creatorUserName = creator.UserName
            });
        }

        return BadRequest(ModelState);
    }

    // Join
    public async Task<IActionResult> Join(int id)
    {
        var studyGroup = await _context.StudyGroups
            .Include(sg => sg.GroupMembers)
            .FirstOrDefaultAsync(sg => sg.Id == id);
        
        if (studyGroup == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        
        // Check if group has reached the max number of members
        if (studyGroup.GroupMembers.Count >= studyGroup.MaxGroupMembers)
        {
            return BadRequest("The group has reached the maximum number of members.");
        }

        // Check if user is already in the group
        if (studyGroup.GroupMembers.Any(m => m.ApplicationUserId == user.Id))
        {
            return BadRequest("You are already a member of this group.");
        }
        
        var userStudyGroup = new UserStudyGroup
        {
            ApplicationUserId = user.Id,
            ApplicationUser = user,
            StudyGroupId = id,
            StudyGroup = studyGroup
        };

        _context.UserStudyGroups.Add(userStudyGroup);
        await _context.SaveChangesAsync();

        return Json(new { userName = user.UserName });
    }
    
    // Delete 
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var studyGroup = await _context.StudyGroups
            .Include(sg => sg.GroupMembers)
            .FirstOrDefaultAsync(sg => sg.Id == id);

        if (studyGroup == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Make sure the current user is the group creator
        if (studyGroup.CreatorId != userId)
        {
            return Unauthorized("Only the group creator can delete the group.");
        }
        
        _context.UserStudyGroups.RemoveRange(studyGroup.GroupMembers);
        _context.StudyGroups.Remove(studyGroup);

        await _context.SaveChangesAsync();
    
        return Json(new { success = true, groupId = id });
    }


}