using JoliPet.Models;
using JoliPet.Shared;

namespace JoliPet.Services;

public class PetService : IPetService
{
    private readonly INotificationService _notificationService;
    
    public  PetService(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task UpdateMoodAsync(Pet pet, int newMood)
    {
        if (pet.Status == Constants.StatusDead)
        {
            return;
        }
        
        pet.LastInteractionAt = DateTime.UtcNow;

        if (newMood <= 0)
        {
            pet.Mood = 0;
            pet.Status = Constants.StatusDead;
            string message = $"Pet {pet.Name} died without your love...";
            await _notificationService.AddNotificationAsync(pet.UserId, message);
        }
        else if (newMood >= Constants.MaxMood)
        {
            pet.Mood = Constants.MaxMood;
            pet.Status = Constants.StatusDead;
            string message = $"Pet {pet.Name} burst because of your love...";
            await _notificationService.AddNotificationAsync(pet.UserId, message);
        }
        else
        {
            pet.Mood = newMood;
        }
    }

    public async Task UpdateExperienceAsync(Pet pet, int amount)
    {
        if (pet.Status == Constants.StatusDead || amount == 0)
        {
            return;
        }
        
        int currentLevel = pet.Level;
        int currentXp = pet.Experience + amount;

        while (currentXp < 0)
        {
            if (currentLevel <= 1)
            {
                currentLevel = 1;
                currentXp = 0;
                break;
            }

            currentLevel--;
            currentXp += currentLevel * Constants.XpPerLevel;
        }
        
        int xpNeeded = currentLevel * Constants.XpPerLevel;
        while (currentXp >= xpNeeded)
        {
            currentXp -= xpNeeded;
            currentLevel++;
            xpNeeded = currentLevel * Constants.XpPerLevel;
        }

        if (currentLevel > pet.Level)
        {
            string message = $"{pet.Name} reached the level {currentLevel}!";
            await _notificationService.AddNotificationAsync(pet.UserId, message);
        }
        
        pet.Level = currentLevel;
        pet.Experience = currentXp;
    }
}