using JoliPet.DTOs;
using JoliPet.Models;
using JoliPet.Services;
using JoliPet.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InteractionsController : ControllerBase
{
    private readonly JoliPetContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMessageAnalyzerService _messageAnalyzer;
    private readonly IPetService _petService;

    public InteractionsController(JoliPetContext context, ICurrentUserService currentUser,
        IMessageAnalyzerService messageAnalyzer,  IPetService petService)
    {
        _context = context;
        _currentUser = currentUser;
        _messageAnalyzer = messageAnalyzer;
        _petService = petService;
    }
    
    [HttpPost("talk")]
    public async Task<ActionResult<MyPetDto>> TalkToPet([FromBody] MessageDto m)
    {
        var userId = _currentUser.GetCurrentUserId();

        var pet = await _context.Pets
            .Include(pet => pet.PetType)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Status == Constants.StatusAlive);

        if (pet == null)
        {
            return NotFound();
        }
        
        var minutesPassed = (DateTime.UtcNow - pet.LastInteractionAt).TotalMinutes;
        var decayedMood = Functions.CalculateCurrentMood(
            pet.Mood, minutesPassed, pet.PetType.BaseEquilibrium, 
            pet.PetType.DecayConstant, pet.PetType.CriticalThreshold, pet.PetType.CriticalDecayRate);

        var totalWeight = await _messageAnalyzer.CalculateMessageWeightAsync(m.Message);
        var moodChange =
            Functions.CalculateWordImpact(totalWeight, pet.PetType.Volatility, pet.PetType.EmotionalInertia);
        
        await _petService.UpdateMoodAsync(pet, (int)(decayedMood + moodChange));
        await _petService.UpdateExperienceAsync(pet, totalWeight);
        
        await  _context.SaveChangesAsync();

        var result = new MyPetDto
        {
            Id = pet.Id,
            Name = pet.Name,
            Mood = pet.Mood,
        };
        
        return Ok(result);
    }
}