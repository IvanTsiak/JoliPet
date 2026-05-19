using JoliPet.DTOs;
using JoliPet.Models;
using JoliPet.Shared;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Services;

public class BattleService : IBattleService
{
    private readonly JoliPetContext _context;
    private readonly IPetService _petService;

    public BattleService(JoliPetContext context, IPetService petService)
    {
        _context = context;
        _petService = petService;
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
            .Include(p => p.PetType).Include(pet => pet.User)
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

        if (isVictory)
        {
            damage = Functions.CalculateDamage(winProbability, rngModifier);
            healing = Functions.CalculateHealing(damage, rngModifier);

            await _petService.UpdateMoodAsync(attacker, attacker.Mood + healing);
            await _petService.UpdateMoodAsync(defender, attacker.Mood - damage);
            
            await _petService.UpdateExperienceAsync(attacker, Constants.XpForWinBattle);
            await _petService.UpdateExperienceAsync(defender, Constants.XpForLoseBattle);
        }
        else
        {
            damage = Functions.CalculateDamage(1 - winProbability, rngModifier);
            healing = Functions.CalculateHealing(damage, rngModifier);
            
            await _petService.UpdateMoodAsync(attacker, attacker.Mood - damage);
            await _petService.UpdateMoodAsync(defender, defender.Mood + healing);
            
            await _petService.UpdateExperienceAsync(attacker, Constants.XpForLoseBattle);
            await _petService.UpdateExperienceAsync(defender, Constants.XpForWinBattle);
        }

        await _context.SaveChangesAsync();

        var result = new BattleResultDto
        {
            IsWinner = isVictory,
            DamageDealtOrTaken = damage,
            AttackerFinalMood = attacker.Mood,
            DefenderFinalMood = defender.Mood,
            DefenderPetName = defender.Name,
            DefenderUserName = defender.User.Username
        };
        
        return result;
    }
}