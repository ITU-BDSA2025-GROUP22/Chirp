using Chirp.Infrastructure;

namespace Chirp.Infrastructure;

public record CheepViewModel(string Author, string Message, string Timestamp);

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
    public List<CheepViewModel> GetCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
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
    
    public List<CheepViewModel> GetCheeps(int page)
    {
        return _cheepRepository.GetCheeps(page);
    }
    
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        return _cheepRepository.GetCheepsByAuthor(author, page);
    }
}
