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
    private readonly ChirpContext _context;
    private readonly IInputSanitizer _inputSanitizer;

    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();
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
        ChirpContext context,
        IInputSanitizer inputSanitizer
        )
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _context = context;
        _inputSanitizer = inputSanitizer;
    }
    
    public Task<IActionResult> OnGetAsync([FromQuery] int? pageNumber)
    {
        CurrentPage = pageNumber ?? 1;
        
        Cheeps = _cheepRepository.GetCheeps(CurrentPage);
        
        int totalCheeps = _cheepRepository.GetTotalCheepCount();
        
        HasNextPage = (CurrentPage * 32) < totalCheeps;
        
        return Task.FromResult<IActionResult>(Page());
    }

    public async Task<IActionResult> OnPostAsync()
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
    
        var sanitizedText = _inputSanitizer.SanitizeCheepText(CheepText);
        
        var author = _authorRepository.GetAuthorByName(userName);

        if (author == null)
        {
            var newAuthor = new Author
            {
                Username = userName,
                Email = userEmail,
                Password = Guid.NewGuid().ToString(),
                Cheeps = new List<Cheep>()
            };
        
            _authorRepository.CreateAuthor(newAuthor);
            author = newAuthor;
        }

        var newCheep = new Cheep
        {
            Author = author,
            Text = sanitizedText,
            TimeStamp = DateTime.UtcNow
        };

        _cheepRepository.AddCheep(newCheep);

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}