#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ChirpContext _db;

    public DeleteModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ChirpContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
    }

    public bool RequirePassword { get; private set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound("Unable to load user.");

        //If the user has a password, we demand it as authentication (doesnt work with external login like github)
        RequirePassword = await _userManager.HasPasswordAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound("Unable to load user.");

        RequirePassword = await _userManager.HasPasswordAsync(user);

        if (RequirePassword)
        {
            if (string.IsNullOrWhiteSpace(Input?.Password))
            {
                ModelState.AddModelError(string.Empty, "Password is required.");
                return Page();
            }

            var ok = await _userManager.CheckPasswordAsync(user, Input.Password);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Incorrect password.");
                return Page();
            }
        }

        //Delete userdata (Authors/Cheeps/follow-relations), matched on username/Email
        var username = user.UserName;
        var email = user.Email;

        //Get relations such that follow links are wiped completely
        var author = await _db.Authors
            .Include(a => a.Cheeps)
            .Include(a => a.Followers)
            .Include(a => a.Following)
            .SingleOrDefaultAsync(a => a.Username == username || (email != null && a.Email == email));

        if (author != null)
        {
            //Clear many-to-many links (otherwise causes issue with joint rows)
            author.Followers.Clear();
            author.Following.Clear();

            //Deletes cheeps
            _db.Cheeps.RemoveRange(author.Cheeps);

            //Deletes Author
            _db.Authors.Remove(author);

            await _db.SaveChangesAsync();
        }

        //Deletes identity and logins/tokens
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var msg = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException("Error deleting user: " + msg);
        }

        //Logs out and redirects to front page.
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Public", new { area = "" });
    }
}