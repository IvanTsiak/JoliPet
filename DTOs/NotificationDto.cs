namespace JoliPet.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}