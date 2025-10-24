using Chirp.Core;
using System.Collections.Generic; // Nødvendig for List

namespace Chirp.Infrastructure;

public interface IAuthorRepository
{
    Author? GetAuthorByName(string name);
    
    List<Author> GetAuthor(int page, int pageSize = 32);
}