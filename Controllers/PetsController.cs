using JoliPet.DTOs;
using JoliPet.Models;
using JoliPet.Services;
using JoliPet.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController : ControllerBase
{
    private readonly JoliPetContext _context;
    private readonly ICurrentUserService _currentUser;
    public PetsController(JoliPetContext context,  ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<PetTypeDto>>> PetTypes()
    {
        var types = await _context.PetTypes
            .Select(p => new PetTypeDto
            {
                Id = p.Id,
                Name = p.Name
            })
            .ToListAsync();
        
        return Ok(types);
    }

    [HttpGet("my")]
    public async Task<ActionResult<MyPetDto>> MyPet()
    {
        var userId = _currentUser.GetCurrentUserId();
        var pet = await _context.Pets
            .Include(pet => pet.PetType)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Status == Constants.StatusAlive);

        if (pet == null)
        {
            return  NotFound(new { message = "Pet not found" });
        }
        
        var minutePassed = (DateTime.UtcNow - pet.LastInteractionAt).TotalMinutes;
        double currentMood = Functions.CalculateCurrentMood(
            pet.Mood,
            minutePassed,
            pet.PetType.BaseEquilibrium, 
            pet.PetType.DecayConstant, 
            pet.PetType.CriticalThreshold, 
            pet.PetType.CriticalDecayRate);

        pet.ApplyMoodChange(currentMood);
        await _context.SaveChangesAsync();

        var result = new MyPetDto
        {
            Id = pet.Id,
            Name = pet.Name,
            Mood = pet.Mood,
        };

        return Ok(result);
    }

    public async Task<IActionResult> CreatePet([FromBody] CreatePetDto petDto)
    {
        var userId = _currentUser.GetCurrentUserId();

        if (_context.Pets.Any(p => p.UserId == userId && p.Status == Constants.StatusAlive))
        {
            return BadRequest(new { message = "You already have a pet" });
        }
        
        var petType = await _context.PetTypes.FindAsync(petDto.TypeId);

        if (petType == null)
        {
            return NotFound(new { message = "Pet type not found" });
        }

        var newPet = new Pet
        {
            UserId = userId,
            Name = petDto.Name,
            PetTypeId = petDto.TypeId,
            
            Experience = 0,
            Level = 0,
            Mood = (int)petType.BaseEquilibrium,
            Status = Constants.StatusAlive,
            
            LastInteractionAt = DateTime.UtcNow,
        };
        
        _context.Pets.Add(newPet);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/abandon")]
    public async Task<IActionResult> AbandonPet([FromRoute] int id)
    {
        var userId = _currentUser.GetCurrentUserId();
        var pet = await _context.Pets
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (pet == null)
        {
            return NotFound(new { message = "Pet not found" });
        }

        if (pet.Status == Constants.StatusDead)
        {
            return BadRequest(new { message = "Pet is dead" });
        }

        pet.Status = Constants.StatusDead;
        pet.LastInteractionAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
}