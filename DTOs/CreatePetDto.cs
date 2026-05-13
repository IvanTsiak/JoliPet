namespace JoliPet.DTOs;

public class CreatePetDto
{
    public int TypeId { get; set; }
    public required string Name { get; set; }
}