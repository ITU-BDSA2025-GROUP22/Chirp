using Chirp.Core;
using Chirp.Infrastructure;

namespace Chirp.Infrastructure;

/// <summary>
/// The cheep view model
/// </summary>
public record CheepViewModel(string Author, string Message, string Timestamp, int CheepId);

/// <summary>
/// The expanded cheep view model
/// </summary>
public record ExpandedCheepViewModel(
    int CheepId,
    string Author, 
    string Message, 
    string Timestamp,
    int LikeCount,
    int DislikeCount,
    bool UserHasLiked,
    bool UserHasDisliked
    );

/// <summary>
/// The cheep service interface
/// </summary>
public interface ICheepService
{
    /// <summary>
    /// Gets the cheeps using the specified page
    /// </summary>
    /// <param name="page">The page</param>
    /// <param name="currentUserId">The current user id</param>
    /// <returns>A list of expanded cheep view model</returns>
    public List<ExpandedCheepViewModel> GetCheeps(int page, int? currentUserId);
    /// <summary>
    /// Gets the cheeps from author using the specified author
    /// </summary>
    /// <param name="author">The author</param>
    /// <param name="page">The page</param>
    /// <param name="currentUserId">The current user id</param>
    /// <returns>A list of expanded cheep view model</returns>
    public List<ExpandedCheepViewModel> GetCheepsFromAuthor(string author, int page, int? currentUserId);
    /// <summary>
    /// Likes the cheep using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <param name="authorId">The author id</param>
    public void LikeCheep(int cheepId, int authorId);
    /// <summary>
    /// Dislikes the cheep using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <param name="authorId">The author id</param>
    public void DislikeCheep(int cheepId, int authorId);
    /// <summary>
    /// Gets the author from cheep id using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The author</returns>
    public Author? GetAuthorFromCheepId(int cheepId);
}

/// <summary>
/// The cheep service class
/// </summary>
/// <seealso cref="ICheepService"/>
public class CheepService : ICheepService
{
    // depend on interface, never concrete classes, SOLID principles
    /// <summary>
    /// The cheep repository
    /// </summary>
    private readonly ICheepRepository _cheepRepository;
    /// <summary>
    /// The like repository
    /// </summary>
    private readonly ILikeRepository _likeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheepService"/> class
    /// </summary>
    /// <param name="cheepRepository">The cheep repository</param>
    /// <param name="likeRepository">The like repository</param>
    public CheepService(ICheepRepository cheepRepository, ILikeRepository likeRepository)
    {
        _cheepRepository = cheepRepository;
        _likeRepository = likeRepository;
    }
    
    /// <summary>
    /// Gets the cheeps using the specified page
    /// </summary>
    /// <param name="page">The page</param>
    /// <param name="currentUserId">The current user id</param>
    /// <returns>A list of expanded cheep view model</returns>
    public List<ExpandedCheepViewModel> GetCheeps(int page, int? currentUserId = null)
    {
        var simpleCheeps = _cheepRepository.GetCheeps(page);
        
        return EnrichWithExpandedData(simpleCheeps, currentUserId);
    }

    /// <summary>
    /// Enriches the with expanded data using the specified simple cheeps
    /// </summary>
    /// <param name="simpleCheeps">The simple cheeps</param>
    /// <param name="currentUserId">The current user id</param>
    /// <returns>A list of expanded cheep view model</returns>
    private List<ExpandedCheepViewModel> EnrichWithExpandedData(
        List<CheepViewModel> simpleCheeps, 
        int? currentUserId)
    {
        return simpleCheeps.Select(cheep => new ExpandedCheepViewModel(
            cheep.CheepId,
            cheep.Author,
            cheep.Message,
            cheep.Timestamp,
            _likeRepository.GetLikeCount(cheep.CheepId),
            _likeRepository.GetDislikeCount(cheep.CheepId),
            currentUserId.HasValue && _likeRepository.HasUserLiked(currentUserId.Value, cheep.CheepId),
            currentUserId.HasValue && _likeRepository.HasUserDisliked(currentUserId.Value, cheep.CheepId)
        )).ToList();
    }

    /// <summary>
    /// Gets the cheeps from author using the specified author
    /// </summary>
    /// <param name="author">The author</param>
    /// <param name="page">The page</param>
    /// <param name="currentUserId">The current user id</param>
    /// <returns>A list of expanded cheep view model</returns>
    public List<ExpandedCheepViewModel> GetCheepsFromAuthor(string author, int page, int? currentUserId = null)
    {
        var simpleCheeps = _cheepRepository.GetCheepsByAuthor(author, page);
        return EnrichWithExpandedData(simpleCheeps, currentUserId);
    }

    /// <summary>
    /// Likes the cheep using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <param name="authorId">The author id</param>
    public void LikeCheep(int cheepId, int authorId)
    {
        // Check if user already interacted with this cheep
        var existingLike = _likeRepository.GetByAuthorAndCheep(authorId, cheepId);
        
        if (existingLike != null)
        {
            if (existingLike.IsLike)
            {
                // Already liked - remove it (toggle off)
                _likeRepository.RemoveLike(existingLike);
                return;
            }
            else
            {
                // Was dislike - remove it first, then add like
                _likeRepository.RemoveLike(existingLike);
            }
        }
        
        // Add new like
        var like = new Like
        {
            CheepId = cheepId,
            AuthorId = authorId,
            IsLike = true
        };
        
        _likeRepository.AddLike(like);
    }
    
    /// <summary>
    /// Dislikes the cheep using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <param name="authorId">The author id</param>
    public void DislikeCheep(int cheepId, int authorId)
    {
        var existingLike = _likeRepository.GetByAuthorAndCheep(authorId, cheepId);
        
        if (existingLike != null)
        {
            if (!existingLike.IsLike)
            {
                // Already disliked - remove it (toggle off)
                _likeRepository.RemoveLike(existingLike);
                return;
            }
            else
            {
                // Was like - remove it first, then add dislike
                _likeRepository.RemoveLike(existingLike);
            }
        }
        
        // Add new dislike
        var dislike = new Like
        {
            CheepId = cheepId,
            AuthorId = authorId,
            IsLike = false
        };
        
        _likeRepository.AddLike(dislike);
    }

    /// <summary>
    /// Gets the author from cheep id using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The author</returns>
    public Author? GetAuthorFromCheepId(int cheepId)
    {
        var cheep = _cheepRepository.GetCheepByCheepId(cheepId);
        return cheep.Author;
    }
}
