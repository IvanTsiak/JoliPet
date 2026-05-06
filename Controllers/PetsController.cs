using JoliPet.DTOs;
using JoliPet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController : ControllerBase
{
    private readonly JoliPetContext _context;
    public PetsController(JoliPetContext context)
    {
        _context = context;
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
    
}