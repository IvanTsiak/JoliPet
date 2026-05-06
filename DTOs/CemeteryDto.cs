namespace JoliPet.DTOs;

public class CemeteryDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string UserName { get; set; }
    public required string PetType { get; set; }
    public int Level { get; set; }
    public DateTime DiedAt  { get; set; }
}