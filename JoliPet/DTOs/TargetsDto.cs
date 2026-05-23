namespace JoliPet.DTOs;

public class TargetsDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string PetOwner { get; set; }
    public int Level { get; set; }
    public required string PetType { get; set; }
}