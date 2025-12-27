using Chirp.Core;

namespace Chirp.Infrastructure;

public interface ILikeRepository
{
    public void AddLike(Like like);
    public void RemoveLike(Like like);
    public Like? GetByAuthorAndCheep(int authorId, int cheepId);
    public int GetLikeCount(int cheepId);
    public int GetDislikeCount(int cheepId);
    public bool HasUserLiked(int authorId, int cheepId);
    public bool HasUserDisliked(int authorId, int cheepId);
}