using Chirp.Core;

namespace Chirp.Infrastructure;

public interface ICheepRepository
{
    void AddCheep(Cheep cheep);
    List<CheepViewModel> GetCheeps(int page, int pageSize = 32);
    List<CheepViewModel> GetCheepsByAuthor(string author, int page, int pageSize = 32);
    List<CheepViewModel> GetCheepsForTimeline(string user, int page, int pageSize = 32);
    public Cheep? GetCheepByCheepId(int cheepId);
    int GetTotalCheepCount();
}