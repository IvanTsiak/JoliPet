namespace JoliPet.DTOs;

public class BattleResultDto
{
    public bool IsWinner { get; set; }
    public int DamageDealtOrTaken { get; set; }
    public int HealTakenByAttackerOrDefender { get; set; }
    public int XpGained { get; set; }
    public int AttackerFinalMood { get; set; }
    public int DefenderFinalMood { get; set; }
}