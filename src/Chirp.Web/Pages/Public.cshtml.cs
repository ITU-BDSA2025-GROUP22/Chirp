using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure;
using System.Security.Claims;

namespace Chirp.Web.Pages;

/// <summary>
/// The public model class
/// </summary>
/// <seealso cref="PageModel"/>
public class PublicModel : PageModel
{
    /// <summary>
    /// The cheep repository
    /// </summary>
    private readonly ICheepRepository _cheepRepository;
    /// <summary>
    /// The author repository
    /// </summary>
    private readonly IAuthorRepository _authorRepository;
    /// <summary>
    /// The like repository
    /// </summary>
    private readonly ILikeRepository _likeRepository;
    /// <summary>
    /// The cheep service
    /// </summary>
    private readonly ICheepService _cheepService;
    /// <summary>
    /// The context
    /// </summary>
    private readonly ChirpContext _context;

    /// <summary>
    /// Gets or sets the value of the cheeps
    /// </summary>
    public List<ExpandedCheepViewModel> Cheeps { get; set; } = new List<ExpandedCheepViewModel>();
    
    /// <summary>
    /// Gets or sets the value of the following
    /// </summary>
    public List<string> Following { get; set; } = new List<string>(); 

    /// <summary>
    /// Gets or sets the value of the current page
    /// </summary>
    public int CurrentPage { get; set; } = 1;
    /// <summary>
    /// Gets or sets the value of the has next page
    /// </summary>
    public bool HasNextPage { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the cheep text
    /// </summary>
    [BindProperty]
    [Required]
    [StringLength(160, ErrorMessage = "Maximum length is 160")]
    [Display(Name = "Cheep Text")]
    public string CheepText { get; set; } = "";

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicModel"/> class
    /// </summary>
    /// <param name="cheepRepository">The cheep repository</param>
    /// <param name="authorRepository">The author repository</param>
    /// <param name="likeRepository">The like repository</param>
    /// <param name="cheepService">The cheep service</param>
    /// <param name="context">The context</param>
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
    
    /// <summary>
    /// Ons the get using the specified page number
    /// </summary>
    /// <param name="pageNumber">The page number</param>
    /// <returns>A task containing the action result</returns>
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
    
    /// <summary>
    /// Ons the post
    /// </summary>
    /// <returns>The action result</returns>
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
    
    /// <summary>
    /// Ons the post like using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The action result</returns>
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
    /// <summary>
    /// Ons the post dislike using the specified cheep id
    /// </summary>
    /// <param name="cheepId">The cheep id</param>
    /// <returns>The action result</returns>
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