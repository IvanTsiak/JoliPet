using JoliPet.DTOs;
using JoliPet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JoliPet.Shared;

namespace JoliPet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CemeteryController : ControllerBase
{
    private readonly JoliPetContext _context;

    public CemeteryController(JoliPetContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CemeteryDto>>> GetCemeteries()
    {
        var diedPets = await _context.Pets
            .Where(p => p.Status == Constants.StatusDead)
            .Select(p => new CemeteryDto
            {
                Id = p.Id,
                Name = p.Name,
                UserName = p.User.Username,
                PetType = p.PetType.Name,
                Level = p.Level,
                DiedAt = p.LastInteractionAt
            })
            .ToListAsync();

        return Ok(diedPets);
    }
}