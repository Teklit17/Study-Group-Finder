using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SG_Finder.Data;
using SG_Finder.Models;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetEvents()
    {
        var events = _context.Events.ToList();
        return Ok(events);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddEvent([FromBody] Event newEvent)
    {
        if (ModelState.IsValid)
        {
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return Ok();
        }
        return BadRequest();
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event updatedEvent)
    {
        var existingEvent = await _context.Events.FindAsync(id);
        if (existingEvent == null)
        {
            return NotFound();
        }

        existingEvent.Title = updatedEvent.Title;
        existingEvent.Description = updatedEvent.Description;
        existingEvent.Location = updatedEvent.Location;
        existingEvent.Start = updatedEvent.Start;
        existingEvent.End = updatedEvent.End;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var existingEvent = await _context.Events.FindAsync(id);
        if (existingEvent == null)
        {
            return NotFound();
        }

        _context.Events.Remove(existingEvent);
        await _context.SaveChangesAsync();
        return Ok();
    }
}