using Chirp.Core;

namespace Chirp.Infrastructure;

/// <summary>
/// The like repository interface
/// </summary>
public interface ILikeRepository
{
    /// <summary>
    /// Adds the like using the specified like
    /// </summary>
    /// <param name="like">The like</param>
    public void AddLike(Like like);
    /// <summary>
    /// Removes the like using the specified like
    /// </summary>
    /// <param name="like">The like</param>
    public void RemoveLike(Like like);
    /// <summary>
    /// Gets the by author and cheep using the specified author id
    /// </summary>
    /// <param name="authorId">The author id</param>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The like</returns>
    public Like? GetByAuthorAndCheep(int authorId, int cheepId);
    /// <summary>
    /// Gets the like count using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The int</returns>
    public int GetLikeCount(int cheepId);
    /// <summary>
    /// Gets the dislike count using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The int</returns>
    public int GetDislikeCount(int cheepId);
    /// <summary>
    /// Hases the user liked using the specified author id
    /// </summary>
    /// <param name="authorId">The author id</param>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The bool</returns>
    public bool HasUserLiked(int authorId, int cheepId);
    /// <summary>
    /// Hases the user disliked using the specified author id
    /// </summary>
    /// <param name="authorId">The author id</param>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The bool</returns>
    public bool HasUserDisliked(int authorId, int cheepId);
}