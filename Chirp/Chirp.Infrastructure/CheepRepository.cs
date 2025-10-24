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
        var cheeps = new List<CheepViewModel>();
        /*
        int offset = (page - 1) * pageSize;
        string sqlQuery = @"SELECT u.username, m.text, m.pub_date 
                     FROM ""message"" m 
                     JOIN ""user"" u ON m.author_id = u.user_id 
                     WHERE u.username = @author
                     ORDER BY m.pub_date DESC
                     LIMIT @pageSize OFFSET @offset";*/
    
        return cheeps;
    }
    
    private static string UnixTimeStampToDateTimeString(DateTime dateTime)
    {
        // Unix timestamp is seconds past epoch
        
        dateTime = dateTime.ToLocalTime();
        return dateTime.ToString("MM/dd/yy H:mm:ss");
      
    }
}