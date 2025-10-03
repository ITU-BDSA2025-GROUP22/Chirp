using Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
}

public class CheepService : ICheepService
{
    private readonly DBFacade _db;

    public CheepService()
    {
        _db = new DBFacade();
    }
    public List<CheepViewModel> GetCheeps(int page)
    {
        return _db.GetPagedCheeps(page);
    }
    

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        return _db.GetPagedCheepsByAuthor(author, page);
    }

   

}
