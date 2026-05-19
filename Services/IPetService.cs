using JoliPet.Models;

namespace JoliPet.Services;

public interface IPetService
{
    Task UpdateMoodAsync(Pet pet, int newMood);
    Task UpdateExperienceAsync(Pet pet, int xpChange);
}