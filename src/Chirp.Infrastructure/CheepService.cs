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
    
    public List<ExpandedCheepViewModel> GetCheepsFromAuthor(string author, int page, int? currentUserId = null)
    {
        var simpleCheeps = _cheepRepository.GetCheepsByAuthor(author, page);
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
}
