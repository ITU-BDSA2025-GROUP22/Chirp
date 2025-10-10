using Chirp.Core;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

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