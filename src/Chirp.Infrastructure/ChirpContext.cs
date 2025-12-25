using Chirp.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Like> Likes { get; set; }

    public ChirpContext(DbContextOptions<ChirpContext> options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cheep>().Property(c => c.Text).HasMaxLength(160);
        
        modelBuilder.Entity<Author>()
            .HasMany(a => a.Following)
            .WithMany(a => a.Followers)
            .UsingEntity(j => j.ToTable("AuthorFollowers"));
        
        modelBuilder.Entity<Like>()
            .HasOne(l => l.Cheep)
            .WithMany(c => c.Likes)
            .HasForeignKey(l => l.CheepId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Like>()
            .HasOne(l => l.Author)
            .WithMany()
            .HasForeignKey(l => l.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Ensure one author can only like/dislike a cheep once
        modelBuilder.Entity<Like>()
            .HasIndex(l => new { l.CheepId, l.AuthorId })
            .IsUnique();
    }
}