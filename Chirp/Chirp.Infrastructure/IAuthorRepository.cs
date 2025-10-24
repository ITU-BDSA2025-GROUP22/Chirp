using Chirp.Core;

namespace Chirp.Infrastructure;

public interface IAuthorRepository
{
    Author? GetAuthorByName(string name);
    Author? GetAuthorByEmail(string email);
    List<Author> GetAuthor(int page, int pageSize = 32);
}