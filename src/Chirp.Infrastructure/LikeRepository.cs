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
        
        var query = _context.Likes
            .Include(l => l.Cheep)
            .Where(l => l.Cheep.CheepId == cheepId)
    }

    public void RemoveLike(Like like)
    {
        _context.Likes.Remove(like);
        _context.SaveChanges();
    }
}