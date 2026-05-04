namespace JoliPet.Models;

public class PetType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Something { get; set; }

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}