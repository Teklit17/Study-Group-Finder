using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SG_Finder.Data;
using SG_Finder.Models;

namespace SG_Finder.Controllers;

[Authorize]
public class StudyGroupController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudyGroupController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager
    )
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var studyGroups = _context
            .StudyGroups.Include(sg => sg.GroupMembers)
            .ThenInclude(ug => ug.ApplicationUser)
            // test
            .Include(sg => sg.PendingMembers)
            .ThenInclude(ug => ug.ApplicationUser)
            .ToList();

        return View(studyGroups);
    }

    // Create
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
                ApplicationUser = creator,
                IsApproved = true,
            };

            studyGroup.GroupMembers.Add(userStudyGroup);

            _context.StudyGroups.Add(studyGroup);
            await _context.SaveChangesAsync();

            return Json(
                new
                {
                    id = studyGroup.Id,
                    groupName = studyGroup.GroupName,
                    groupDescription = studyGroup.GroupDescription,
                    maxGroupMembers = studyGroup.MaxGroupMembers,
                    creatorUserName = creator.UserName,

                    creatorId = studyGroup.CreatorId,
                }
            );
        }

        return BadRequest(ModelState);
    }

    // Join
    public async Task<IActionResult> Join(int id)
    {
        var studyGroup = await _context
            .StudyGroups.Include(sg => sg.GroupMembers)
            .Include(sg => sg.PendingMembers)
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

        if (studyGroup.GroupMembers.Count >= studyGroup.MaxGroupMembers)
        {
            return BadRequest("The group has reached the maximum number of members.");
        }

        if (studyGroup.GroupMembers.Any(m => m.ApplicationUserId == user.Id))
        {
            return BadRequest("You are already a member.");
        }

        if (studyGroup.PendingMembers.Any(m => m.ApplicationUserId == user.Id))
        {
            return Json(new { pendingRequest = true });
        }

        var userStudyGroup = new UserStudyGroup
        {
            ApplicationUserId = user.Id,
            ApplicationUser = user,
            StudyGroupId = id,
            StudyGroup = studyGroup,
            IsApproved = !studyGroup.RequiresApproval,
        };

        if (studyGroup.RequiresApproval)
        {
            studyGroup.PendingMembers.Add(userStudyGroup);
        }
        else
        {
            studyGroup.GroupMembers.Add(userStudyGroup);
        }

        await _context.SaveChangesAsync();

        return Json(
            new { userName = user.UserName, requiresApproval = studyGroup.RequiresApproval }
        );
    }

    // Approve member request
    [HttpPost]
    public async Task<IActionResult> ApproveMember(int groupId, string userId)
    {
        var studyGroup = await _context
            .StudyGroups.Include(sg => sg.PendingMembers)
            .FirstOrDefaultAsync(sg => sg.Id == groupId);

        if (studyGroup == null)
        {
            return NotFound();
        }

        var pendingMember = studyGroup.PendingMembers.FirstOrDefault(m =>
            m.ApplicationUserId == userId
        );
        if (pendingMember == null)
        {
            return NotFound();
        }

        pendingMember.IsApproved = true;
        studyGroup.PendingMembers.Remove(pendingMember);
        studyGroup.GroupMembers.Add(pendingMember);

        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    // Reject member request
    [HttpPost]
    public async Task<IActionResult> RejectMember(int groupId, string userId)
    {
        var studyGroup = await _context
            .StudyGroups.Include(sg => sg.PendingMembers)
            .FirstOrDefaultAsync(sg => sg.Id == groupId);

        if (studyGroup == null)
        {
            return NotFound();
        }

        var pendingMember = studyGroup.PendingMembers.FirstOrDefault(m =>
            m.ApplicationUserId == userId
        );
        if (pendingMember == null)
        {
            return NotFound();
        }

        studyGroup.PendingMembers.Remove(pendingMember);

        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    // Leave group
    [HttpPost]
    public async Task<IActionResult> Leave(int id)
    {
        var studyGroup = await _context
            .StudyGroups.Include(sg => sg.GroupMembers)
            .Include(sg => sg.PendingMembers)
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

        var userStudyGroup =
            studyGroup.GroupMembers.FirstOrDefault(m => m.ApplicationUserId == user.Id)
            ?? studyGroup.PendingMembers.FirstOrDefault(m => m.ApplicationUserId == user.Id);

        if (userStudyGroup == null)
        {
            return BadRequest("You are not a member of this group.");
        }

        if (userStudyGroup.IsApproved)
        {
            studyGroup.GroupMembers.Remove(userStudyGroup);
        }
        else
        {
            studyGroup.PendingMembers.Remove(userStudyGroup);
        }

        await _context.SaveChangesAsync();

        return Json(new { userName = user.UserName, groupName = studyGroup.GroupName });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveMember(int groupId, string userId)
    {
        var studyGroup = await _context
            .StudyGroups.Include(sg => sg.GroupMembers)
            .FirstOrDefaultAsync(sg => sg.Id == groupId);

        if (studyGroup == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || studyGroup.CreatorId != user.Id)
        {
            return Unauthorized();
        }

        var member = studyGroup.GroupMembers.FirstOrDefault(m => m.ApplicationUserId == userId);
        if (member == null)
        {
            return NotFound();
        }

        studyGroup.GroupMembers.Remove(member);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    // Delete
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var studyGroup = await _context
            .StudyGroups.Include(sg => sg.GroupMembers)
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

    // Search
    [HttpGet]
    public IActionResult Search(string query)
    {
        var lowerCaseQuery = query.ToLower();
        var studyGroups = _context
            .StudyGroups.Where(studyGroup =>
                studyGroup.GroupName.ToLower().Contains(lowerCaseQuery)
                || studyGroup.GroupDescription.ToLower().Contains(lowerCaseQuery)
            )
            .Include(studyGroup => studyGroup.GroupMembers)
            .ThenInclude(groupMember => groupMember.ApplicationUser)
            .ToList();

        return PartialView("_GroupListPartial", studyGroups);
    }
}
