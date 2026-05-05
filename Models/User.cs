using System.ComponentModel.DataAnnotations;

namespace JoliPet.Models;

public class User
{
    public int Id { get; set; }
    [MaxLength(50)]
    public required string Username { get; set; }
    [MaxLength(50)]
    public required string Email { get; set; }
    [MaxLength(260)]
    public required string Password { get; set; }
    public DateTime JoinedDate { get; set; }
    
    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}