using Microsoft.EntityFrameworkCore;

namespace JoliPet.Models;

public class JoliPetContext : DbContext
{
    public JoliPetContext(DbContextOptions<JoliPetContext> options) : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Pet> Pets { get; set; }
    public virtual DbSet<PetType> PetTypes { get; set; }
    public virtual DbSet<Word> Words { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.JoinedDate)
            .HasDefaultValueSql("GETUTCDATE()");
        
        modelBuilder.Entity<Pet>()
            .Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
        
        modelBuilder.Entity<Notification>()
            .Property(n => n.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}