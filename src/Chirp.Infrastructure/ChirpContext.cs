using Chirp.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

/// <summary>
/// The chirp context class
/// </summary>
/// <seealso cref="IdentityDbContext{ApplicationUser}"/>
public class ChirpContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Gets or sets the value of the cheeps
    /// </summary>
    public DbSet<Cheep> Cheeps { get; set; }
    /// <summary>
    /// Gets or sets the value of the authors
    /// </summary>
    public DbSet<Author> Authors { get; set; }
    /// <summary>
    /// Gets or sets the value of the likes
    /// </summary>
    public DbSet<Like> Likes { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChirpContext"/> class
    /// </summary>
    /// <param name="options">The options</param>
    public ChirpContext(DbContextOptions<ChirpContext> options) : base(options) {}
    
    /// <summary>
    /// Ons the model creating using the specified model builder
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
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