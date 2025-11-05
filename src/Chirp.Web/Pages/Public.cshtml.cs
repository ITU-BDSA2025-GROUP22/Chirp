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

    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();

    [BindProperty]
    public string CheepText { get; set; } = "";

    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, ChirpContext context)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _context = context;
    }
    
    public Task<IActionResult> OnGetAsync(int page = 1, int pageSize = 32)
    {
        Cheeps = _cheepRepository.GetCheeps(page, pageSize);
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
            Text = CheepText,
            TimeStamp = DateTime.UtcNow
        };

        _cheepRepository.AddCheep(newCheep);

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}