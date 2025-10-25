using Chirp.Infrastructure;
using Chirp.Core;
using Microsoft.EntityFrameworkCore; 
using Xunit;
using System.Linq;

namespace Chirp.Tests;

public class AuthorRepositoryTests
{   
    private readonly IAuthorRepository _repository;
    private readonly ChirpContext _context;

    public AuthorRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ChirpContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Author") 
            .Options;
        
        _context = new ChirpContext(options);
        
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        
        _context.Authors.AddRange(
            new Author { Username = "Adrian", Password = "123", Email = "adrian@test.com" },
            new Author { Username = "Børge", Password = "123", Email = "borge@test.com" }
        );
        
        _context.SaveChanges();

        _repository = new AuthorRepository(_context);
    }

    [Fact]
    public void CreateAuthorTest()
    {
        var testAuthor = new Author { Username = "Jones", Password = "abc", Email = "jones@test.com"};
        _repository.CreateAuthor(testAuthor);
        _context.SaveChanges();
        
        Assert.NotNull(testAuthor);
        Assert.Equal("Jones", testAuthor.Username);
        Assert.Equal("abc", testAuthor.Password);
        Assert.Equal("jones@test.com", testAuthor.Email);
    }
        
    [Fact]
    public void GetAuthorByNameTest()
    {
        var author = _repository.GetAuthorByName("Adrian");
        
        Assert.NotNull(author);
        Assert.Equal("Adrian", author.Username);
    }
    
    
}