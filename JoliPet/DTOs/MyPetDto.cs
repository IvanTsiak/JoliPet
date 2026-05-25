namespace JoliPet.DTOs;

public class MyPetDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Mood { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
}