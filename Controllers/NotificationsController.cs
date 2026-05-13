using JoliPet.DTOs;
using JoliPet.Models;
using JoliPet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly JoliPetContext _context;
    private readonly ICurrentUserService _currentUser;
    
    public NotificationsController(JoliPetContext context,  ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
    {
        var userId = _currentUser.GetCurrentUserId();
        
        var unreadNotifications = await _context.Notifications
            .Include(n => n.User)
            .Where(n => n.UserId == userId && !n.IsRead)
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
        var userId = _currentUser.GetCurrentUserId();
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (notification == null)
        {
            return NotFound(new { message = "Notification not found" });
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}