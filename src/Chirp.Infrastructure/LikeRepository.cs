using Chirp.Core;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

/// <summary>
/// The like repository class
/// </summary>
/// <seealso cref="ILikeRepository"/>
public class LikeRepository : ILikeRepository
{
    /// <summary>
    /// The context
    /// </summary>
    private readonly ChirpContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="LikeRepository"/> class
    /// </summary>
    /// <param name="context">The context</param>
    public LikeRepository (ChirpContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Adds the like using the specified like
    /// </summary>
    /// <param name="like">The like</param>
    public void AddLike(Like like)
    {
        _context.Likes.Add(like);
        _context.SaveChanges();
    }

    /// <summary>
    /// Removes the like using the specified like
    /// </summary>
    /// <param name="like">The like</param>
    public void RemoveLike(Like like)
    {
        _context.Likes.Remove(like);
        _context.SaveChanges();
    }
    
    /// <summary>
    /// Method for finding the Like linked to the specified
    /// authorId and cheepId. Likes are unique, so there can never
    /// exist two instances of the same like.
    /// </summary>
    /// <param name="authorId">authorId</param>
    /// <param name="cheepId">cheepId</param>
    /// <returns name="Like">Like</returns>
    public Like? GetByAuthorAndCheep(int authorId, int cheepId)
    {
        return _context.Likes
            .FirstOrDefault(l => l.AuthorId == authorId && l.CheepId == cheepId);
    }
    
    /// <summary>
    /// Gets the like count using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The int</returns>
    public int GetLikeCount(int cheepId)
    {
        return _context.Likes
            .Count(l => l.CheepId == cheepId && l.IsLike);
    }
    
    /// <summary>
    /// Gets the dislike count using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The int</returns>
    public int GetDislikeCount(int cheepId)
    {
        return _context.Likes
            .Count(l => l.CheepId == cheepId && !l.IsLike);
    }
    
    /// <summary>
    /// Hases the user liked using the specified author id
    /// </summary>
    /// <param name="authorId">The author id</param>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The bool</returns>
    public bool HasUserLiked(int authorId, int cheepId)
    {
        return _context.Likes
            .Any(l => l.AuthorId == authorId && l.CheepId == cheepId && l.IsLike);
    }
    
    /// <summary>
    /// Hases the user disliked using the specified author id
    /// </summary>
    /// <param name="authorId">The author id</param>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The bool</returns>
    public bool HasUserDisliked(int authorId, int cheepId)
    {
        return _context.Likes
            .Any(l => l.AuthorId == authorId && l.CheepId == cheepId && !l.IsLike);
    }
}