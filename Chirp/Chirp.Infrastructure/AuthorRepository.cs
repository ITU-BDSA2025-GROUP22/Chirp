//using Microsoft.Data.Sqlite;
using Chirp.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpContext _context;

    public AuthorRepository (ChirpContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Returns the specified author and formats the page
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public List<CheepViewModel> GetAuthor(int page, int pageSize = 32)
    {   
        //SKRIV PASSENDE SQL KODE TIL DENNE METODE
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
    
    private static string UnixTimeStampToDateTimeString(DateTime dateTime)
    {
        dateTime = dateTime.ToLocalTime();
        return dateTime.ToString("MM/dd/yy H:mm:ss");

    }
}