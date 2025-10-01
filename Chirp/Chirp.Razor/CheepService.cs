using Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    private readonly DBFacade _db;

    public CheepService()
    {
        _db = new DBFacade();
    }
    public List<CheepViewModel> GetCheeps()
    {
        return _db.GetAllCheeps();
    }
    

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return _db.GetCheepsByAuthor(author);
    }

   

}
