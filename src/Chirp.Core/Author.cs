namespace Chirp.Core;

/// <summary>
/// The author class
/// </summary>
public class Author
{
    
    /// <summary>
    /// Gets or sets the value of the author id
    /// </summary>
    public int AuthorId { get; set; } 
    /// <summary>
    /// Gets or sets the value of the username
    /// </summary>
    public required string Username { get; set; }
    /// <summary>
    /// Gets or sets the value of the email
    /// </summary>
    public required string Email { get; set; }
    /// <summary>
    /// Gets or sets the value of the password
    /// </summary>
    public required string Password { get; set; }
    /// <summary>
    /// Gets or sets the value of the cheeps
    /// </summary>
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
    
    /// <summary>
    /// Gets or sets the value of the following
    /// </summary>
    public List<Author> Following { get; set; } = new();
    /// <summary>
    /// Gets or sets the value of the followers
    /// </summary>
    public List<Author> Followers { get; set; } = new();
}