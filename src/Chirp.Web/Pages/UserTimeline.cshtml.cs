using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _cheepRepo;
    private readonly IAuthorRepository _authorRepo;
    
    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();
    public List<string> Following { get; set; } = new List<string>();
    public bool IsFollowing { get; set; } = false;

    public UserTimelineModel(ICheepRepository cheepRepo, IAuthorRepository authorRepo)
    {
        _cheepRepo = cheepRepo;
        _authorRepo = authorRepo;
    }

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