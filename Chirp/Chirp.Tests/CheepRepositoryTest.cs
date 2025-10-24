using Chirp.Infrastructure;
using Chirp.Core;
using Microsoft.EntityFrameworkCore; 

namespace Chirp.Tests;

public class CheepRepositoryTests
{   
    private readonly CheepRepository _repository;

    public CheepRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ChirpContext>()
            .UseInMemoryDatabase(databaseName: "TestDb") 
            .Options;
        
        var context = new ChirpContext(options);
        
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        var author1 = new Author { Username = "Adrian", Password = "qwert", Email = "adrian@test.com" };
        var author2 = new Author { Username = "Børge", Password = "123", Email = "borge@test.com" };
        context.Authors.AddRange(author1, author2);

        context.Cheeps.AddRange(
            new Cheep { Author = author1, Text = "Adrians cheep", TimeStamp = 100 },
            new Cheep { Author = author2, Text = "Børges cheep", TimeStamp = 200 }
        );
        context.SaveChanges();

        _repository = new CheepRepository(context);
    }

    [Fact]
    public void TestGetPagedCheeps()
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
}