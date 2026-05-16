using JoliPet.DTOs;
using JoliPet.Models;
using JoliPet.Services;
using JoliPet.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BattlesController : ControllerBase
{
    private readonly JoliPetContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IBattleService _battleService;
    
    public  BattlesController(JoliPetContext context,  ICurrentUserService currentUser,  IBattleService battleService)
    {
        _context = context;
        _currentUser = currentUser;
        _battleService = battleService;
    }

    [HttpGet("targets")]
    public async Task<ActionResult<IEnumerable<TargetsDto>>> Targets()
    {
        var userId = _currentUser.GetCurrentUserId();
        
        var userPet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId && p.Status == Constants.StatusAlive);

        if (userPet == null)
        {
            return NotFound(new { message = "You do not have alive pet to use this" });
        }

        var targets = await _context.Pets
            .Where(p => p.Level >= userPet.Level - Constants.MaxDifferenceBetweenLevel
                        && p.Level <= userPet.Level + Constants.MaxDifferenceBetweenLevel
                        && p.Id != userPet.Id
                        && p.Status == Constants.StatusAlive)
            .Select(p => new TargetsDto
            {
                Id = p.Id,
                Name = p.Name,
                PetOwner = p.User.Username,
                Level = p.Level,
                PetType = p.PetType.Name
            })
            .ToListAsync();
        
        return Ok(targets);
    }

    [HttpPost("{id}/attack")]
    public async Task<ActionResult<BattleResultDto>> Attack([FromRoute] int defenderPetId)
    {
        try
        {
            var userId = _currentUser.GetCurrentUserId();
            var result = await _battleService.ExecuteBattleAsync(userId, defenderPetId);

            return Ok(result);
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }
    }
}