namespace JoliPet.DTOs;

public class BattleResultDto
{
    public bool IsWinner { get; set; }
    public int DamageDealtOrTaken { get; set; }
    public int AttackerFinalMood { get; set; }
    public int DefenderFinalMood { get; set; }
    public required string DefenderPetName { get; set; }
    public required string DefenderUserName { get; set; }
}