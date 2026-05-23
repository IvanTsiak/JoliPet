using System.Security.Claims;
using JoliPet.DTOs;
using JoliPet.Models;
using JoliPet.Services;
using JoliPet.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JoliPetContext _context;
    private readonly ICurrentUserService _currentUserService;
    
    public AuthController(JoliPetContext context,  ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto r)
    {
        bool emailExists = await _context.Users.AnyAsync(u => u.Email == r.Email);
        if (emailExists)
        {
            return BadRequest("Email already exists");
        }
        
        bool usernameExists = await _context.Users.AnyAsync(u => u.Username == r.Username);
        if (usernameExists)
        {
            return BadRequest("Username already exists");
        }
        
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(r.Password);

        var newUser = new User
        {
            Username = r.Username,
            Email = r.Email,
            Password = passwordHash
        };
        
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        
        return Ok(new { message = "User created" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto l)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == l.Email);

        if (user != null && BCrypt.Net.BCrypt.Verify(l.Password, user.Password))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            
            return Ok(new { message = "User authorized", userId = user.Id, username = user.Username });
        }
        
        return Unauthorized(new  { message = "Email or password is incorrect" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    [HttpGet("whoami")]
    public async Task<ActionResult<WhoAmIDto>> WhoAmI()
    {
        var userId = _currentUserService.GetCurrentUserId();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Unauthorized(new { message = "User not found" });
        }
        
        var result = new WhoAmIDto
        {
            Id = user.Id,
            Name = user.Username,
            Email = user.Email
        };
        
        return Ok(result);

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