using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Chirp.Web.Pages;

/// <summary>
/// The user timeline model class
/// </summary>
/// <seealso cref="PageModel"/>
public class UserTimelineModel : PageModel
{
    /// <summary>
    /// The cheep repo
    /// </summary>
    private readonly ICheepRepository _cheepRepo;
    /// <summary>
    /// The author repo
    /// </summary>
    private readonly IAuthorRepository _authorRepo;
    
    /// <summary>
    /// Gets or sets the value of the cheeps
    /// </summary>
    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();
    /// <summary>
    /// Gets or sets the value of the following
    /// </summary>
    public List<string> Following { get; set; } = new List<string>();
    /// <summary>
    /// Gets or sets the value of the is following
    /// </summary>
    public bool IsFollowing { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserTimelineModel"/> class
    /// </summary>
    /// <param name="cheepRepo">The cheep repo</param>
    /// <param name="authorRepo">The author repo</param>
    public UserTimelineModel(ICheepRepository cheepRepo, IAuthorRepository authorRepo)
    {
        _cheepRepo = cheepRepo;
        _authorRepo = authorRepo;
    }

    /// <summary>
    /// Ons the get using the specified author
    /// </summary>
    /// <param name="author">The author</param>
    /// <param name="page">The page</param>
    /// <returns>The action result</returns>
    public ActionResult OnGet(string author, [FromQuery] int page = 1)
    {
        var userName = User.Identity?.Name;

        if (User.Identity?.IsAuthenticated == true && userName != null)
        {
            Following = _authorRepo.GetFollowing(userName);
        }

        if (User.Identity?.IsAuthenticated == true && userName == author)
        {
            Cheeps = _cheepRepo.GetCheepsForTimeline(author, page); 
        }
        else 
        {
            Cheeps = _cheepRepo.GetCheepsByAuthor(author, page);

            if (User.Identity?.IsAuthenticated == true)
            {
                IsFollowing = Following.Contains(author);
            }
        }

        return Page();
    }

    /// <summary>
    /// Ons the post follow using the specified author
    /// </summary>
    /// <param name="author">The author</param>
    /// <returns>The action result</returns>
    public ActionResult OnPostFollow(string author)
    {
        var userName = User.Identity?.Name;
        
        if (userName != null) 
        {
            var exists = _authorRepo.GetAuthorByName(userName);
            
            if (exists == null)
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "no-email-saved";
                
                _authorRepo.CreateAuthor(new Author 
                { 
                    Username = userName, 
                    Email = email,
                    Cheeps = new List<Cheep>(),
                    Following = new List<Author>(),
                    Followers = new List<Author>(),
                    Password = "None"
                });
            }

            _authorRepo.FollowAuthor(userName, author);
        }
        return RedirectToPage(new { author = author });
    }

    /// <summary>
    /// Ons the post unfollow using the specified author
    /// </summary>
    /// <param name="author">The author</param>
    /// <returns>The action result</returns>
    public ActionResult OnPostUnfollow(string author)
    {
        var userName = User.Identity?.Name;
        if (userName != null)
        {
            _authorRepo.UnfollowAuthor(userName, author);
        }
        return RedirectToPage(new { author = author });
    }
}