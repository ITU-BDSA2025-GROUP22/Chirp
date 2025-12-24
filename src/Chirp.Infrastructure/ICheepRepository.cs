using Chirp.Core;

namespace Chirp.Infrastructure;

public interface ICheepRepository
{
    void AddCheep(Cheep cheep);
    List<CheepViewModel> GetCheeps(int page, int pageSize = 32);
    List<CheepViewModel> GetCheepsByAuthor(string author, int page, int pageSize = 32);
    List<CheepViewModel> GetCheepsForTimeline(string user, int page, int pageSize = 32);
    int GetTotalCheepCount();
}