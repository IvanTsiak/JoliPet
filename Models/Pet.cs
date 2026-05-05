using System.ComponentModel.DataAnnotations;

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
}