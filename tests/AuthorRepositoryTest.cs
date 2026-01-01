using Chirp.Infrastructure;
using Chirp.Core;
using Microsoft.EntityFrameworkCore; 

namespace Chirp.Tests;

/// <summary>
/// The author repository tests class
/// </summary>
public class AuthorRepositoryTests
{   
    /// <summary>
    /// The repository
    /// </summary>
    private readonly IAuthorRepository _repository;
    /// <summary>
    /// The context
    /// </summary>
    private readonly ChirpContext _context;

    /// <summary>
    /// Sets up an in-memory database and seeds it with test authors.
    /// </summary>
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

    /// <summary>
    /// Tests that CreateAuthor(Author author) successfully adds an author.
    /// </summary>
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
        
    /// <summary>
    /// Tests GetAuthorByName(string name) for an existing author.
    /// </summary>
    [Fact]
    public void GetAuthorByNameTest()
    {
        var author = _repository.GetAuthorByName("Adrian");
        
        Assert.NotNull(author);
        Assert.Equal("Adrian", author.Username);
    }

    /// <summary>
    /// Tests GetAuthorByEmail(string email) for an existing author.
    /// </summary>
    [Fact]
    public void GetAuthorByEmailTest()
    {
        var author = _repository.GetAuthorByEmail("adrian@test.com");
        
        Assert.NotNull(author);
        Assert.Equal("Adrian", author.Username);
    }
    
    /// <summary>
    /// Tests GetAuthors(int page, int pageSize) returns the correct list.
    /// </summary>
    [Fact]
    public void GetAuthorsTest()
    {
        var authors = _repository.GetAuthors(1, 32);

        Assert.Equal(2, authors.Count);
        Assert.Contains(authors, a => a.Username == "Adrian");
    }
}