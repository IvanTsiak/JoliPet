namespace JoliPet.DTOs;

public class GuestInfoDto
{
    public int PetTypesCount { get; set;  }
    public required List<string> PetTypesName { get; set; }
    public int UserCount { get; set; }
    public int AlivePetsCount { get; set; }
    public int DeadPetsCount { get; set; }
}