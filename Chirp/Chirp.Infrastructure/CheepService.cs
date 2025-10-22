using Chirp.Infrastructure;

namespace Chirp.Infrastructure;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
}

public class CheepService : ICheepService
{
    // depend on interface, never concrete classes, SOLID principles
    private readonly ICheepRepository _db;

    public CheepService(ICheepRepository db)
    {
        _db = db;
    }
    
    public List<CheepViewModel> GetCheeps(int page)
    {
        return _db.GetCheeps(page);
    }
    
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        return _db.GetCheepsByAuthor(author, page);
    }
}
