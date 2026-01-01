using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

/// <summary>
/// The cheep repository class
/// </summary>
/// <seealso cref="ICheepRepository"/>
public class CheepRepository : ICheepRepository
{
    /// <summary>
    /// The context
    /// </summary>
    private readonly ChirpContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheepRepository"/> class
    /// </summary>
    /// <param name="context">The context</param>
    public CheepRepository (ChirpContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets the cheeps using the specified page
    /// </summary>
    /// <param name="page">The page</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>The query</returns>
    public List<CheepViewModel> GetCheeps(int page, int pageSize = 32)
    {   
        int offset = (page - 1) * pageSize;

        var query = _context.Cheeps
            .Include(c => c.Author)
            .Include(c => c.Likes)
            .OrderByDescending(c => c.TimeStamp)
            .Select(c => new CheepViewModel(
                c.Author.Username,
                c.Text,
                UnixTimeStampToDateTimeString(c.TimeStamp),
                c.CheepId
            ))
            .Skip(offset)
            .Take(pageSize)
            .ToList();
 
        return query;
    }
    
    /// <summary>
    /// Adds the cheep using the specified cheep
    /// </summary>
    /// <param name="cheep">The cheep</param>
    public void AddCheep(Cheep cheep)
    {
        _context.Cheeps.Add(cheep);
        _context.SaveChanges();
    }
    
    /// <summary>
    /// Gets the cheeps by author using the specified author
    /// </summary>
    /// <param name="author">The author</param>
    /// <param name="page">The page</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>The query</returns>
    public List<CheepViewModel> GetCheepsByAuthor(string author, int page, int pageSize = 32)
    {
        int offset = (page - 1) * pageSize;

        var query = _context.Cheeps
            .Include(c => c.Author)
            .Include(c => c.Likes)
            .Where(c => c.Author.Username == author)
            .OrderByDescending(c => c.TimeStamp)
            .Select(c => new CheepViewModel(
                c.Author.Username,
                c.Text,
                UnixTimeStampToDateTimeString(c.TimeStamp),
                c.CheepId
            ))
            .Skip(offset)
            .Take(pageSize)
            .ToList();
    
        return query;
    }

    /// <summary>
    /// Gets the cheeps for timeline using the specified user
    /// </summary>
    /// <param name="user">The user</param>
    /// <param name="page">The page</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>The query</returns>
    public List<CheepViewModel> GetCheepsForTimeline(string user, int page, int pageSize = 32)
    {
        int offset = (page - 1) * pageSize;

        var author = _context.Authors
            .Include(a => a.Following)
            .FirstOrDefault(a => a.Username == user);

        if (author == null) return new List<CheepViewModel>();

        var followingNames = author.Following.Select(a => a.Username).ToList();
        followingNames.Add(user);

        var query = _context.Cheeps
            .Include(c => c.Author)
            .Where(c => followingNames.Contains(c.Author.Username))
            .OrderByDescending(c => c.TimeStamp)
            .Select(c => new CheepViewModel(
                c.Author.Username,
                c.Text,
                UnixTimeStampToDateTimeString(c.TimeStamp),
                c.CheepId
            ))
            .Skip(offset)
            .Take(pageSize)
            .ToList();

        return query;
    }
    
    /// <summary>
    /// Gets the total cheep count
    /// </summary>
    /// <returns>The int</returns>
    public int GetTotalCheepCount()
    {
        return _context.Cheeps.Count();
    }
    
    /// <summary>
    /// Unixes the time stamp to date time string using the specified date time
    /// </summary>
    /// <param name="dateTime">The date time</param>
    /// <returns>The string</returns>
    private static string UnixTimeStampToDateTimeString(DateTime dateTime)
    {
        dateTime = dateTime.ToLocalTime();
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

    /// <summary>
    /// Gets the cheep by cheep id using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The cheep</returns>
    public Cheep? GetCheepByCheepId(int cheepId)
    {
        return _context.Cheeps.FirstOrDefault(c => c.CheepId == cheepId);
    }
}