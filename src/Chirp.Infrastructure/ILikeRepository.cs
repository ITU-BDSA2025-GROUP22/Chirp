using Chirp.Core;

namespace Chirp.Infrastructure;

public interface ILikeRepository
{
    public void AddLike(Like like, int cheepId, int authorId);
    public void RemoveLike(Like like);
}