using System.ComponentModel.DataAnnotations;

namespace JoliPet.Models;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [MaxLength(200)]
    public required string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public virtual User User { get; set; }
}