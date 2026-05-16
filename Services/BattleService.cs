using JoliPet.DTOs;
using JoliPet.Models;
using JoliPet.Shared;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Services;

public class BattleService : IBattleService
{
    private readonly JoliPetContext _context;

    public BattleService(JoliPetContext context)
    {
        _context = context;
    }

    public async Task<BattleResultDto> ExecuteBattleAsync(int attackerId, int defenderId)
    {
        if (attackerId == defenderId)
        {
            throw new InvalidOperationException();
        }

        var attacker = await _context.Pets
            .Include(p => p.PetType)
            .FirstOrDefaultAsync(p => p.Id == attackerId && p.Status == Constants.StatusAlive);

        var defender = await _context.Pets
            .Include(p => p.PetType)
            .FirstOrDefaultAsync(p => p.Id == defenderId && p.Status == Constants.StatusAlive);

        if (attacker == null || defender == null)
        {
            throw new InvalidOperationException();
        }

        double cpAttacker = Functions.CalculateCombatPower(attacker.Mood, attacker.Level,
            attacker.PetType.BaseEquilibrium, attacker.PetType.Volatility, attacker.PetType.EmotionalInertia);
        double cpDefender = Functions.CalculateCombatPower(defender.Mood, defender.Level,
            defender.PetType.BaseEquilibrium, defender.PetType.Volatility, defender.PetType.EmotionalInertia);

        double winProbability = Functions.CalculateWinProbability(cpAttacker, cpDefender);
        double diceRoll = Random.Shared.NextDouble();

        bool isVictory = diceRoll <= winProbability;

        double rngModifier = Random.Shared.NextDouble() * (Constants.RngModifierTo - Constants.RngModifierFrom) + Constants.RngModifierFrom;

        int damage, healing;
        // Потім реалізувати отримання досвіду та підвищення рівня

        if (isVictory)
        {
            damage = Functions.CalculateDamage(winProbability, rngModifier);
            healing = Functions.CalculateHealing(damage, rngModifier);

            attacker.ApplyMoodChange(attacker.Mood + healing);
            defender.ApplyMoodChange(defender.Mood - damage);
        }
        else
        {
            damage = Functions.CalculateDamage(1 - winProbability, rngModifier);
            healing = Functions.CalculateHealing(damage, rngModifier);

            attacker.ApplyMoodChange(attacker.Mood - damage);
            defender.ApplyMoodChange(defender.Mood + healing);
        }

        await _context.SaveChangesAsync();

        var result = new BattleResultDto
        {
            IsWinner = isVictory,
            DamageDealtOrTaken = damage,
            AttackerFinalMood = attacker.Mood,
            DefenderFinalMood = defender.Mood,
        };
        
        return result;
    }
}