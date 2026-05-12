using JoliPet.DTOs;
using JoliPet.Models;
using JoliPet.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JoliPetContext _context;
    
    public AuthController(JoliPetContext context)
    {
        _context = context;
    }
    
    [HttpGet("guest-info")]
    public async Task<ActionResult<GuestInfoDto>> GuestInfo()
    {
        var petTypesCount = await _context.PetTypes.CountAsync();
        var petTypesName = await _context.PetTypes.Select(p => p.Name).ToListAsync();
        var userCount = await _context.Users.CountAsync();
        var alivePets = await _context.Pets.Where(p => p.Status == Constants.StatusAlive).CountAsync();
        var deadPets = await _context.Pets.Where(p => p.Status == Constants.StatusDead).CountAsync();

        var result = new GuestInfoDto
        {
            PetTypesCount = petTypesCount,
            PetTypesName = petTypesName,
            UserCount = userCount,
            AlivePetsCount = alivePets,
            DeadPetsCount = deadPets
        };

        return Ok(result);
    }
}