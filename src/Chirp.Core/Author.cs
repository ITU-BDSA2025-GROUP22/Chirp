namespace Chirp.Core;

public class Author
{
    
    public int AuthorId { get; set; } 
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
    
    public List<Author> Following { get; set; } = new();
    public List<Author> Followers { get; set; } = new();
}
}