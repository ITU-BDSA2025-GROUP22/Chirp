using Chirp.Infrastructure;
using Chirp.Core;
using Microsoft.EntityFrameworkCore; 
using Xunit;
using System.Linq;

namespace Chirp.Tests;

public class CheepRepositoryTests
{   
    private readonly CheepRepository _repository;
    private readonly ChirpContext _context;

    /// <summary>
    /// Sets up an in-memory database with 2 authors and 2 cheeps before each test.
    /// </summary>
    public CheepRepositoryTests()
    {
        string pattern = "MM-dd-yy";
        var options = new DbContextOptionsBuilder<ChirpContext>()
            .UseInMemoryDatabase(databaseName: "TestDb") 
            .Options;
        
        _context = new ChirpContext(options);
        
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        
        var author1 = new Author { Username = "Adrian", Password = "qwert", Email = "adrian@test.com" };
        var author2 = new Author { Username = "Børge", Password = "123", Email = "borge@test.com" };
        _context.Authors.AddRange(author1, author2);

        _context.Cheeps.AddRange(
            
            new Cheep { Author = author1, Text = "Adrians cheep", TimeStamp = DateTime.Parse("2023-08-01 13:14:58") },
            new Cheep { Author = author2, Text = "Børges cheep", TimeStamp = DateTime.Parse("2024-09-10 03:44:38") }
        );
        _context.SaveChanges();

        _repository = new CheepRepository(_context);
    }

    /// <summary>
    /// Tests that GetCheeps returns all cheeps, ordered newest first.
    /// </summary>
    [Fact]
    public void GetCheepsTest()
    {
        var cheeps = _repository.GetCheeps(1, 32);

        Assert.Equal(2, cheeps.Count);
        Assert.Equal("Børge", cheeps[0].Author);
    }

    /// <summary>
    /// Tests that GetCheepsByAuthor only returns cheeps from the specified author.
    /// </summary>
    [Fact]
    public void GetCheepsByAuthorTest()
    {
        var cheeps = _repository.GetCheepsByAuthor("Adrian", 1, 32);
        
        Assert.Single(cheeps);
        Assert.Equal("Adrian", cheeps[0].Author);
    }

    /// <summary>
    /// Tests that AddCheep correctly adds a new cheep to the database.
    /// </summary>
    [Fact]
    public void AddCheepTest()
    {
        var author = _context.Authors.First(a => a.Username == "Adrian");
        var newCheep = new Cheep 
        { 
            Author = author, 
            Text = "A brand new cheep!", 
            TimeStamp = DateTime.Parse("2026-08-01 13:14:58")
        };

        _repository.AddCheep(newCheep);
        _context.SaveChanges(); 

        var cheeps = _repository.GetCheeps(1, 1);

        Assert.Single(cheeps);
        Assert.Equal("Adrian", cheeps[0].Author);
        Assert.Equal("A brand new cheep!", cheeps[0].Message);
    }
}