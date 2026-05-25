using System;
using System.Linq;
using System.Threading.Tasks;
using JoliPet.Models;
using JoliPet.Shared;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Services;

public class NotificationService : INotificationService
{
    private readonly JoliPetContext _context;
    
    public NotificationService(JoliPetContext context)
    {
        _context = context;
    }

    public async Task AddNotificationAsync(int userId, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        bool needsCleanupSave = false;
        
        var someDaysAgo = DateTime.Now.AddDays(Constants.SomeDaysAgo);

        var tooOldNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.CreatedAt < someDaysAgo)
            .ToListAsync();

        if (tooOldNotifications.Count > 0)
        {
            _context.Notifications.RemoveRange(tooOldNotifications);
            needsCleanupSave = true;
        }

        var excessNotifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip(Constants.MaxNotificationsPerUser)
            .ToListAsync();
        
        if (excessNotifications.Count > 0)
        {
            _context.Notifications.RemoveRange(excessNotifications);
            needsCleanupSave = true;
        }

        if (needsCleanupSave)
        {
            await _context.SaveChangesAsync();
        }
    }
}