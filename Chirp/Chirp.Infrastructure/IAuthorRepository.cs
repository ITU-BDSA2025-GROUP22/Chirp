using Chirp.Core;

namespace Chirp.Infrastructure;

public interface IAuthorRepository
{
    public List<CheepViewModel> GetAuthor(int page, int pageSize = 32);
}