using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

/// <summary>
/// The cheep class
/// </summary>
public class Cheep
{
    /// <summary>
    /// Gets or sets the value of the cheep id
    /// </summary>
    public int CheepId { get; set; } 
    
    /// <summary>
    /// Gets or sets the value of the text
    /// </summary>
    public required string Text { get; set; } 
    /// <summary>
    /// Gets or sets the value of the time stamp
    /// </summary>
    public DateTime TimeStamp { get; set; } 
    
    /// <summary>
    /// Gets or sets the value of the author id
    /// </summary>
    public int AuthorId { get; set; }
    /// <summary>
    /// Gets or sets the value of the author
    /// </summary>
    public required Author Author { get; set; }

    /// <summary>
    /// Gets or sets the value of the likes
    /// </summary>
    public List<Like> Likes { get; set; } = new();
}
