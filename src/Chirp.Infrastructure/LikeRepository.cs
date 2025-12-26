using Chirp.Core;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class LikeRepository : ILikeRepository
{
    private readonly ChirpContext _context;

    public LikeRepository (ChirpContext context)
    {
        _context = context;
    }
    
    public void AddLike(Like like, int cheepId, int authorId)
    {
        // Set the foreign keys
        like.CheepId = cheepId;
        like.AuthorId = authorId;
        _context.Likes.Add(like);
        _context.SaveChanges();
    }

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
    
    public int GetLikeCount(int cheepId)
    {
        return _context.Likes
            .Count(l => l.CheepId == cheepId && l.IsLike);
    }
    
    public int GetDislikeCount(int cheepId)
    {
        return _context.Likes
            .Count(l => l.CheepId == cheepId && !l.IsLike);
    }
    
    public bool HasUserLiked(int authorId, int cheepId)
    {
        return _context.Likes
            .Any(l => l.AuthorId == authorId && l.CheepId == cheepId && l.IsLike);
    }
    
    public bool HasUserDisliked(int authorId, int cheepId)
    {
        return _context.Likes
            .Any(l => l.AuthorId == authorId && l.CheepId == cheepId && !l.IsLike);
    }
}