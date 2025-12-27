using Chirp.Core;
using Chirp.Infrastructure;

namespace Chirp.Infrastructure;

public record CheepViewModel(string Author, string Message, string Timestamp, int CheepId);

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

public interface ICheepService
{
    public List<ExpandedCheepViewModel> GetCheeps(int page, int? currentUserId);
    public List<ExpandedCheepViewModel> GetCheepsFromAuthor(string author, int page, int? currentUserId);
    public void LikeCheep(int cheepId, int authorId);
    public void DislikeCheep(int cheepId, int authorId);
    public Author? GetAuthorFromCheepId(int cheepId);
}

public class CheepService : ICheepService
{
    // depend on interface, never concrete classes, SOLID principles
    private readonly ICheepRepository _cheepRepository;
    private readonly ILikeRepository _likeRepository;

    public CheepService(ICheepRepository cheepRepository, ILikeRepository likeRepository)
    {
        _cheepRepository = cheepRepository;
        _likeRepository = likeRepository;
    }
    
    public List<ExpandedCheepViewModel> GetCheeps(int page, int? currentUserId = null)
    {
        var simpleCheeps = _cheepRepository.GetCheeps(page);
        
        return EnrichWithExpandedData(simpleCheeps, currentUserId);
    }

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

    public List<ExpandedCheepViewModel> GetCheepsFromAuthor(string author, int page, int? currentUserId = null)
    {
        var simpleCheeps = _cheepRepository.GetCheepsByAuthor(author, page);
        return EnrichWithExpandedData(simpleCheeps, currentUserId);
    }

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

    public Author? GetAuthorFromCheepId(int cheepId)
    {
        var cheep = _cheepRepository.GetCheepByCheepId(cheepId);
        return cheep.Author;
    }
}
