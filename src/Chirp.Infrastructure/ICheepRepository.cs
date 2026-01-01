using Chirp.Core;

namespace Chirp.Infrastructure;

/// <summary>
/// The cheep repository interface
/// </summary>
public interface ICheepRepository
{
    /// <summary>
    /// Adds the cheep using the specified cheep
    /// </summary>
    /// <param name="cheep">The cheep</param>
    void AddCheep(Cheep cheep);
    /// <summary>
    /// Gets the cheeps using the specified page
    /// </summary>
    /// <param name="page">The page</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>A list of cheep view model</returns>
    List<CheepViewModel> GetCheeps(int page, int pageSize = 32);
    /// <summary>
    /// Gets the cheeps by author using the specified author
    /// </summary>
    /// <param name="author">The author</param>
    /// <param name="page">The page</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>A list of cheep view model</returns>
    List<CheepViewModel> GetCheepsByAuthor(string author, int page, int pageSize = 32);
    /// <summary>
    /// Gets the cheeps for timeline using the specified user
    /// </summary>
    /// <param name="user">The user</param>
    /// <param name="page">The page</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>A list of cheep view model</returns>
    List<CheepViewModel> GetCheepsForTimeline(string user, int page, int pageSize = 32);
    /// <summary>
    /// Gets the cheep by cheep id using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The cheep</returns>
    public Cheep? GetCheepByCheepId(int cheepId);
    /// <summary>
    /// Gets the total cheep count
    /// </summary>
    /// <returns>The int</returns>
    int GetTotalCheepCount();
}