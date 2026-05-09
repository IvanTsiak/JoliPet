namespace JoliPet.Services;

public interface INotificationService
{
    Task AddNotificationAsync(int userId, string message);
    
}