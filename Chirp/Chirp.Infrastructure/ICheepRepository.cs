using Chirp.Core;

namespace Chirp.Infrastructure;

public interface ICheepRepository
{
    public IEnumerable<Cheep> GetCheeps();
    
    public IEnumerable<Cheep> GetCheepsByAuthor(string author);

    public void AddCheep(Cheep cheep);
}