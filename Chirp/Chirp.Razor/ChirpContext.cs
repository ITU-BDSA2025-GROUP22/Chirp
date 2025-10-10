using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class ChirpContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    public ChirpContext(DbContextOptions<ChirpContext> options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>();
        modelBuilder.Entity<Cheep>();
        
        base.OnModelCreating(modelBuilder);
    }
}

public class Author
{
    public int AuthorId { get; set; } 
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
}

public class Cheep
{
    public int CheepId { get; set; } 
    
    public required string Text { get; set; } 
    public long TimeStamp { get; set; } 
    
    public int AuthorId { get; set; }
    public required Author Author { get; set; } 
}