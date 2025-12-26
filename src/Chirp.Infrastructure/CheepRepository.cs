using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpContext _context;

    public CheepRepository (ChirpContext context)
    {
        _context = context;
    }
    
    public List<CheepViewModel> GetCheeps(int page, int pageSize = 32)
    {   
        int offset = (page - 1) * pageSize;

        var query = _context.Cheeps
            .Include(c => c.Author)
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
    
    public void AddCheep(Cheep cheep)
    {
        _context.Cheeps.Add(cheep);
        _context.SaveChanges();
    }
    
    public List<CheepViewModel> GetCheepsByAuthor(string author, int page, int pageSize = 32)
    {
        int offset = (page - 1) * pageSize;

        var query = _context.Cheeps
            .Include(c => c.Author)
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
    
    public int GetTotalCheepCount()
    {
        return _context.Cheeps.Count();
    }
    
    private static string UnixTimeStampToDateTimeString(DateTime dateTime)
    {
        dateTime = dateTime.ToLocalTime();
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}