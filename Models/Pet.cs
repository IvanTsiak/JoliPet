using System.ComponentModel.DataAnnotations;
using JoliPet.Shared;

namespace JoliPet.Models;

public class Pet
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PetTypeId { get; set; }
    [MaxLength(50)]
    public required string Name { get; set; }
    public int Mood { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastInteractionAt { get; set; }
    
    public virtual PetType PetType { get; set; }
    public virtual User User { get; set; }


    public void ApplyMoodChange(double newMood)
    {
        Mood = Math.Max(Constants.MinMood, (int)newMood);
        
        LastInteractionAt = DateTime.UtcNow;

        if (Mood <= 0)
        {
            Status = Constants.StatusDead;
        }
    }

    public void ChangeExperience(int amount)
    {
        Experience += amount;

        while (Experience < 0)
        {
            if (Level <= 1)
            {
                Level = 1;
                Experience = 0;
                break;
            }

            Level--;
            Experience += Level * Constants.XpPerLevel;
        }

        int xpNeeded = Level * Constants.XpPerLevel;

        while (Experience >= xpNeeded)
        {
            Experience -= xpNeeded;
            Level++;
            
            xpNeeded = Level * Constants.XpPerLevel;
        }
    }
}