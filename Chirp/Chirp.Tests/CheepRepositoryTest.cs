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

    public CheepRepositoryTests()
    {
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
            new Cheep { Author = author1, Text = "Adrians cheep", TimeStamp = 100 },
            new Cheep { Author = author2, Text = "Børges cheep", TimeStamp = 200 }
        );
        _context.SaveChanges();

        _repository = new CheepRepository(_context);
    }

    [Fact]
    public void TestGetCheeps()
    {
        var cheeps = _repository.GetCheeps(1, 32);

        Assert.Equal(2, cheeps.Count);
        Assert.Equal("Børge", cheeps[0].Author);
    }

    [Fact]
    public void TestGetCheepsByAuthor()
    {
        var cheeps = _repository.GetCheepsByAuthor("Adrian", 1, 32);
        
        Assert.Single(cheeps);
        Assert.Equal("Adrian", cheeps[0].Author);
    }

    [Fact]
    public void TestAddCheep()
    {
        var author = _context.Authors.First(a => a.Username == "Adrian");
        var newCheep = new Cheep 
        { 
            Author = author, 
            Text = "En helt ny cheep!", 
            TimeStamp = 999
        };

        _repository.AddCheep(newCheep);
        _context.SaveChanges(); 

        var cheeps = _repository.GetCheeps(1, 1);

        Assert.Single(cheeps);
        Assert.Equal("Adrian", cheeps[0].Author);
        Assert.Equal("En helt ny cheep!", cheeps[0].Message);
    }
}