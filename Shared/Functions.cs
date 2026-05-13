namespace JoliPet.Shared;

public static class Functions
{
    public static double CalculateCurrentMood(
        double mood,
        double minutesPassed,
        double baseEquilibrium,
        double decayConstant,
        double criticalThreshold,
        double criticalDecayRate)
    {
        if (mood <= criticalThreshold)
        {
            return Math.Max(Constants.MinMood, mood - (criticalDecayRate * minutesPassed));
        }

        double expMood = baseEquilibrium + (mood - baseEquilibrium) * Math.Exp(-decayConstant * minutesPassed);

        if (expMood > criticalThreshold)
        {
            return Math.Min(Constants.MinMood, expMood);
        }
        
        double minutesTrans = -Math.Log((criticalThreshold - baseEquilibrium) / (mood - baseEquilibrium)) / decayConstant;
        double remainingMinutes = minutesPassed - minutesTrans;
        double finalMood = criticalThreshold - (criticalDecayRate * remainingMinutes);
        
        return Math.Max(Constants.MinMood, finalMood);
    }

    public static double CalculateWordImpact(int totalWeight, double volatility, double emotionalInertia)
    {
        if (totalWeight == 0)
        {
            return 0;
        }

        if (emotionalInertia == 0)
        {
            emotionalInertia = 1;
        }

        double impact = (volatility / emotionalInertia) * Math.Log(Math.Abs(totalWeight) + 1);

        return Math.Sign(totalWeight) * impact;
    }
} 