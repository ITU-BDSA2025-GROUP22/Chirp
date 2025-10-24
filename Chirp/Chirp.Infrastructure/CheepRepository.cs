//using Microsoft.Data.Sqlite;
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
                UnixTimeStampToDateTimeString(c.TimeStamp)
            ))
            .Skip(offset)
            .Take(pageSize)
            .ToList();
 
        return query;
    }
    
    public void AddCheep(Cheep cheep)
    {
        _context.Cheeps.Add(cheep);
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
                UnixTimeStampToDateTimeString(c.TimeStamp)
            ))
            .Skip(offset)
            .Take(pageSize)
            .ToList();
    
        return query;
    }
    
    private static string UnixTimeStampToDateTimeString(DateTime dateTime)
    {
        // Unix timestamp is seconds past epoch
        
        dateTime = dateTime.ToLocalTime();
        return dateTime.ToString("MM/dd/yy H:mm:ss");
      
    }
}