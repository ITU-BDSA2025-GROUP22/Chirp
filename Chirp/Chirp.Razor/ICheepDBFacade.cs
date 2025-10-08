namespace Chirp.Razor;

public interface ICheepDBFacade
{
    public List<CheepViewModel> GetCheeps(int page, int pageSize = 32);
    public List<CheepViewModel> GetCheepsByAuthor(string author, int page, int pageSize = 32);
}