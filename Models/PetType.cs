using System.ComponentModel.DataAnnotations;

namespace JoliPet.Models;

public class PetType
{
    public int Id { get; set; }
    [MaxLength(50)]
    public required string Name { get; set; }
    public double BaseEquilibrium { get; set; }
    public double DecayConstant { get; set; }
    public double Volatility { get; set; }
    public double EmotionalInertia { get; set; }
    public double CriticalThreshold { get; set; }
    public double CriticalDecayRate { get; set; }
    
    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}