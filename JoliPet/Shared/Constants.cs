namespace JoliPet.Shared;

public static class Constants
{
    public const int MaxMood = 100;
    public const int MinMood = 0;

    public const int SomeDaysAgo = -30;
    public const int MaxNotificationsPerUser = 30;

    public const int StatusAlive = 1;
    public const int StatusDead = 0;

    public const int MaxDifferenceBetweenLevel = 5;
    
    public const double LevelMultiplier = 1.8;
    public const double TypeMultiplier = 5;
    public const double BalanceCoefficient = 0.05;
    public const int MaxDamage = 25;

    public const double RngModifierFrom = 0.5;
    public const double RngModifierTo = 1.0;
    
    public const int XpPerLevel = 100;
    public const int XpForWinBattle = 15;
    public const int XpForLoseBattle = -5;
}