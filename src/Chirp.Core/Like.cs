namespace Chirp.Core;

/// <summary>
/// The like class
/// </summary>
public class Like
{
    /// <summary>
    /// Gets or sets the value of the like id
    /// </summary>
    public int LikeId { get; set; }
    /// <summary>
    /// Gets or sets the value of the cheep id
    /// </summary>
    public int CheepId { get; set; }
    /// <summary>
    /// Gets or sets the value of the author id
    /// </summary>
    public int AuthorId { get; set; }
    /// <summary>
    /// Gets or sets the value of the is like
    /// </summary>
    public bool IsLike { get; set; } // true = like, false = dislike
    
    /// <summary>
    /// Gets or sets the value of the cheep
    /// </summary>
    public Cheep Cheep { get; set; } = null!;
    /// <summary>
    /// Gets or sets the value of the author
    /// </summary>
    public Author Author { get; set; } = null!;
}