using Chirp.Core;

namespace Chirp.Infrastructure;

public interface IAuthorRepository
{
    void CreateAuthor(Author author);
    Author? GetAuthorByName(string name);
    public int? GetAuthorId(Author author);
    Author? GetAuthorByEmail(string email);
    List<Author> GetAuthors(int page, int pageSize = 32);
    void FollowAuthor(string followerName, string followingName);
    void UnfollowAuthor(string followerName, string followingName);
    List<string> GetFollowing(string authorName);
}