using Chirp.Core;

namespace Chirp.Infrastructure;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpContext _context;

    public AuthorRepository (ChirpContext context)
    {
        _context = context;
    }
    
    public Author? GetAuthorByName(string name)
    {
        return _context.Authors.FirstOrDefault(a => a.Username == name);
    }
    
    public Author? GetAuthorByEmail(string email)
    {
        return _context.Authors.FirstOrDefault(a => a.Email == email);
    }
    
    public List<Author> GetAuthor(int page, int pageSize = 32)
    {   
        int offset = (page - 1) * pageSize;

        return _context.Authors
            .OrderBy(a => a.Username) 
            .Skip(offset)
            .Take(pageSize)
            .ToList();
    }
    
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}