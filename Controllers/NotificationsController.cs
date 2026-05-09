using JoliPet.DTOs;
using JoliPet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly JoliPetContext _context;
    
    public NotificationsController(JoliPetContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
    {
        var unreadNotifications = await _context.Notifications
            .Include(n => n.User)
            .Where(n => n.IsRead == false)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                UserName = n.User.Username,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
            })
            .ToListAsync();

        return Ok(unreadNotifications);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> ReadNotification([FromRoute] int id)
    {
        var notification = await _context.Notifications.FindAsync(id);

        if (notification == null)
        {
            return NotFound(new { message = "Not found" });
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}