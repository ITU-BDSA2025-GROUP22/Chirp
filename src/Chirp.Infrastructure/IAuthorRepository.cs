using Chirp.Core;

namespace Chirp.Infrastructure;

/// <summary>
/// The author repository interface
/// </summary>
public interface IAuthorRepository
{
    /// <summary>
    /// Creates the author using the specified author
    /// </summary>
    /// <param name="author">The author</param>
    void CreateAuthor(Author author);
    /// <summary>
    /// Gets the author by name using the specified name
    /// </summary>
    /// <param name="name">The name</param>
    /// <returns>The author</returns>
    Author? GetAuthorByName(string name);
    /// <summary>
    /// Gets the author id using the specified author
    /// </summary>
    /// <param name="author">The author</param>
    /// <returns>The int</returns>
    public int? GetAuthorId(Author author);
    /// <summary>
    /// Gets the author by email using the specified email
    /// </summary>
    /// <param name="email">The email</param>
    /// <returns>The author</returns>
    Author? GetAuthorByEmail(string email);
    /// <summary>
    /// Gets the authors using the specified page
    /// </summary>
    /// <param name="page">The page</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>A list of author</returns>
    List<Author> GetAuthors(int page, int pageSize = 32);
    /// <summary>
    /// Follows the author using the specified follower name
    /// </summary>
    /// <param name="followerName">The follower name</param>
    /// <param name="followingName">The following name</param>
    void FollowAuthor(string followerName, string followingName);
    /// <summary>
    /// Unfollows the author using the specified follower name
    /// </summary>
    /// <param name="followerName">The follower name</param>
    /// <param name="followingName">The following name</param>
    void UnfollowAuthor(string followerName, string followingName);
    /// <summary>
    /// Gets the following using the specified author name
    /// </summary>
    /// <param name="authorName">The author name</param>
    /// <returns>A list of string</returns>
    List<string> GetFollowing(string authorName);
}