using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure;
using System.Security.Claims;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly ICheepService _cheepService;
    private readonly ChirpContext _context;

    public List<ExpandedCheepViewModel> Cheeps { get; set; } = new List<ExpandedCheepViewModel>();
    
    public List<string> Following { get; set; } = new List<string>(); 

    public int CurrentPage { get; set; } = 1;
    public bool HasNextPage { get; set; }
    
    [BindProperty]
    [Required]
    [StringLength(160, ErrorMessage = "Maximum length is 160")]
    [Display(Name = "Cheep Text")]
    public string CheepText { get; set; } = "";

    public PublicModel(
        ICheepRepository cheepRepository, 
        IAuthorRepository authorRepository, 
        ILikeRepository likeRepository,
        ICheepService cheepService,
        ChirpContext context
        )
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _likeRepository = likeRepository;
        _cheepService = cheepService; 
        _context = context;
    }
    
    public Task<IActionResult> OnGetAsync([FromQuery] int? pageNumber)
    {
        CurrentPage = pageNumber ?? 1;
        
        // Get current user ID
        int? currentUserId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var userName = User.Identity.Name;
            if (userName != null)
            {
                var author = _authorRepository.GetAuthorByName(userName);
                if (author != null)
                {
                    currentUserId = author.AuthorId;
                }
                Following = _authorRepository.GetFollowing(userName);
            }
        }
        
        Cheeps = _cheepService.GetCheeps(CurrentPage, currentUserId);
        
        int totalCheeps = _cheepRepository.GetTotalCheepCount();
        HasNextPage = (CurrentPage * 32) < totalCheeps;
        
        return Task.FromResult<IActionResult>(Page());
    }
    
    public IActionResult OnPost()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Unauthorized();
        }
        
        var userName = User.Identity?.Name;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail))
        {
            return RedirectToPage();
        }
    
        var author = _authorRepository.GetAuthorByName(userName);

        if (author == null)
        {
            var newAuthor = new Author
            {
                Username = userName,
                Email = userEmail,
                Cheeps = new List<Cheep>(),
                Following = new List<Author>(),
                Followers = new List<Author>(),
                Password = "None"
            };
        
            _authorRepository.CreateAuthor(newAuthor);
            author = newAuthor;
        }

        var newCheep = new Cheep
        {
            Author = author,
            Text = CheepText,
            TimeStamp = DateTime.UtcNow
        };

        _cheepRepository.AddCheep(newCheep);
        
        return RedirectToPage();
    }
    
    public IActionResult OnPostLike(int cheepId)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return LocalRedirect("/Identity/Account/Login");
        }
        
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            return RedirectToPage();
        }
        
        var author = _authorRepository.GetAuthorByName(userName);
        if (author == null)
        {
            return RedirectToPage();
        }
        
        _cheepService.LikeCheep(cheepId, author.AuthorId);
        
        TempData["SuccessMessage"] = $"✓ Disliked cheep #{cheepId}";
        return RedirectToPage();
    }
    
    // ✅ BONUS: Add dislike handler
    public IActionResult OnPostDislike(int cheepId)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return LocalRedirect("/Identity/Account/Login");
        }
        
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            return RedirectToPage();
        }
        
        var author = _authorRepository.GetAuthorByName(userName);
        if (author == null)
        {
            return RedirectToPage();
        }
        
        _cheepService.DislikeCheep(cheepId, author.AuthorId);
        
        TempData["SuccessMessage"] = $"✓ Disliked cheep #{cheepId}";
        return RedirectToPage();
    }
}