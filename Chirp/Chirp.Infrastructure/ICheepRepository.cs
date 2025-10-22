using Chirp.Core;

namespace Chirp.Infrastructure;

public interface ICheepRepository
{
    public List<CheepViewModel> GetCheeps(int page, int pageSize = 32);
    
    public List<CheepViewModel> GetCheepsByAuthor(string author, int page, int pageSize = 32);

    public void AddCheep(Cheep cheep);
}