using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpContext _context;

    public AuthorRepository (ChirpContext context)
    {
        _context = context;
    }
    
    public void CreateAuthor(Author author)
    {
        _context.Authors.Add(author);
        _context.SaveChanges();
    }
    
    public Author? GetAuthorByName(string name)
    {
        return _context.Authors.FirstOrDefault(a => a.Username == name);
    }
    
    public Author? GetAuthorByEmail(string email)
    {
        return _context.Authors.FirstOrDefault(a => a.Email == email);
    }
    
    public List<Author> GetAuthors(int page, int pageSize = 32)
    {   
        int offset = (page - 1) * pageSize;

        return _context.Authors
            .OrderBy(a => a.Username) 
            .Skip(offset)
            .Take(pageSize)
            .ToList();
    }

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

    public List<string> GetFollowing(string authorName)
    {
        var author = _context.Authors.Include(a => a.Following).FirstOrDefault(a => a.Username == authorName);
        return author?.Following.Select(a => a.Username).ToList() ?? new List<string>();
    }
}