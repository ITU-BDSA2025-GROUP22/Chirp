using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

/// <summary>
/// The author repository class
/// </summary>
/// <seealso cref="IAuthorRepository"/>
public class AuthorRepository : IAuthorRepository
{
    /// <summary>
    /// The context
    /// </summary>
    private readonly ChirpContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorRepository"/> class
    /// </summary>
    /// <param name="context">The context</param>
    public AuthorRepository (ChirpContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Creates the author using the specified author
    /// </summary>
    /// <param name="author">The author</param>
    public void CreateAuthor(Author author)
    {
        _context.Authors.Add(author);
        _context.SaveChanges();
    }
    
    /// <summary>
    /// Gets the author by name using the specified name
    /// </summary>
    /// <param name="name">The name</param>
    /// <returns>The author</returns>
    public Author? GetAuthorByName(string name)
    {
        return _context.Authors.FirstOrDefault(a => a.Username == name);
    }

    /// <summary>
    /// Gets the author id using the specified author
    /// </summary>
    /// <param name="author">The author</param>
    /// <returns>The int</returns>
    public int? GetAuthorId(Author author)
    {
        return author.AuthorId;
    }
    
    /// <summary>
    /// Gets the author by email using the specified email
    /// </summary>
    /// <param name="email">The email</param>
    /// <returns>The author</returns>
    public Author? GetAuthorByEmail(string email)
    {
        return _context.Authors.FirstOrDefault(a => a.Email == email);
    }
    
    /// <summary>
    /// Gets the authors using the specified page
    /// </summary>
    /// <param name="page">The page</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>A list of author</returns>
    public List<Author> GetAuthors(int page, int pageSize = 32)
    {   
        int offset = (page - 1) * pageSize;

        return _context.Authors
            .OrderBy(a => a.Username) 
            .Skip(offset)
            .Take(pageSize)
            .ToList();
    }

    /// <summary>
    /// Follows the author using the specified follower name
    /// </summary>
    /// <param name="followerName">The follower name</param>
    /// <param name="followingName">The following name</param>
    public void FollowAuthor(string followerName, string followingName)
    {
        var follower = _context.Authors.Include(a => a.Following).FirstOrDefault(a => a.Username == followerName);
        var following = _context.Authors.FirstOrDefault(a => a.Username == followingName);

        if (follower != null && following != null)
        {
            if (!follower.Following.Contains(following))
            {
                follower.Following.Add(following);
                _context.SaveChanges();
            }
        }
    }

    /// <summary>
    /// Unfollows the author using the specified follower name
    /// </summary>
    /// <param name="followerName">The follower name</param>
    /// <param name="followingName">The following name</param>
    public void UnfollowAuthor(string followerName, string followingName)
    {
        var follower = _context.Authors.Include(a => a.Following).FirstOrDefault(a => a.Username == followerName);
        var following = _context.Authors.FirstOrDefault(a => a.Username == followingName);

        if (follower != null && following != null)
        {
            follower.Following.Remove(following);
            _context.SaveChanges();
        }
    }

    /// <summary>
    /// Gets the following using the specified author name
    /// </summary>
    /// <param name="authorName">The author name</param>
    /// <returns>A list of string</returns>
    public List<string> GetFollowing(string authorName)
    {
        var author = _context.Authors.Include(a => a.Following).FirstOrDefault(a => a.Username == authorName);
        return author?.Following.Select(a => a.Username).ToList() ?? new List<string>();
    }
}