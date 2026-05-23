using JoliPet.Shared;

namespace JoliPet.Tests;

public class FunctionsTest
{
    [Fact]
    public void CalculateDamage_ShouldReturnZero_WhenProbabilityIsZero()
    {
        double winProbability = 0;
        double rngModifier = 1;
        
        double damage = Functions.CalculateDamage(winProbability, rngModifier);

        Assert.Equal(0, damage);
    }
    
    [Fact]
    public void CalculateWordImpact_ShouldPrevetDivideByZero_WhenInertiaIsZero()
    {
        int weight = 10;
        double volatility = 2;
        double inertia = 0;
        
        double result = Functions.CalculateWordImpact(weight, volatility, inertia);
        double expected = 2 * Math.Log(11);
        
        Assert.Equal(expected, result, precision: 4);
    }
    
    [Fact]
    public void CalculateWordImpact_ShouldReturnNegative_WhenWeightIsNegative()
    {
        int weight = -10;
        double volatility = 2;
        double inertia = 2.5;
        
        double result = Functions.CalculateWordImpact(weight, volatility, inertia);
        
        Assert.True(result < 0);
    }

    [Fact]
    public void CalculateCombatPower_ShouldGiveMaxHarmony_WhenMoodEqualsBaseEquilibrium()
    {
        int mood = 20;
        int worseMood = 50;
        double baseEquilibrium = 20;
        int level = 5;
        double volatility = 1;
        double inertia = 1;
        
        double cp = Functions.CalculateCombatPower(mood, level, baseEquilibrium, volatility, inertia);
        
        double cpWorseMood = Functions.CalculateCombatPower(worseMood, level, baseEquilibrium, volatility, inertia);
        
        Assert.True(cpWorseMood < cp);
    }

    [Fact]
    public void CalculateWinProbability_ShouldReturn50Percent_WhenPowersAreEqual()
    {
        double cpAttacker = 150;
        double cpDefender = 150;
        
        double probability = Functions.CalculateWinProbability(cpAttacker, cpDefender);
        
        Assert.Equal(0.5, probability, precision: 3);
    }

    [Fact]
    public void CalculateCurrentMood_ShouldUseLinearDecay_WhenAlreadyBelowCriticalThreshold()
    {
        double initialMood = 15;
        double minutesPassed = 10;
        double baseEquilibrium = 50;
        double decayConstant = 0.1;
        double criticalThreshold = 20;
        double criticalDecayRate = 0.5;

        double result = Functions.CalculateCurrentMood(
            initialMood, minutesPassed, baseEquilibrium, decayConstant,
            criticalThreshold, criticalDecayRate);
        
        Assert.Equal(10, result, precision: 2);
    }
}