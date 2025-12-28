#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using System.Security.Claims;
using System.Text.Json;


[Authorize]
public class ManageDataModel  : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ChirpContext _db;

    public ManageDataModel(
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

    //Export Userdata
    public async Task<IActionResult> OnPostDownloadAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        
        // Userdata
        var identityPart = new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.PhoneNumber
        };
        var logins = await _userManager.GetLoginsAsync(user);
        var externalLogins = logins.Select(l => new
        {
            l.LoginProvider,
            l.ProviderKey,
            l.ProviderDisplayName
        }).ToList();
        
        // Chirp domain data (project to DTO to avoid cycles)
        var authorExport = await _db.Authors
            .Where(a => a.Username == user.UserName || (user.Email != null && a.Email == user.Email))
            .Select(a => new
            {
                a.AuthorId,
                a.Username,
                a.Email,

                Cheeps = a.Cheeps
                    .OrderByDescending(c => c.TimeStamp)
                    .Select(c => new { c.CheepId, c.Text, c.TimeStamp })
                    .ToList(),

                Following = a.Following.Select(x => x.Username).ToList(),
                Followers = a.Followers.Select(x => x.Username).ToList()
            })
            .SingleOrDefaultAsync();

        var export = new
        {
            ExportedAtUtc = DateTime.UtcNow,
            Identity = identityPart,
            ExternalLogins = externalLogins,
            Chirp = authorExport
        };

        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(export, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var fileName = $"chirp-mydata-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json";
        return File(jsonBytes, "application/json", fileName);
    }


    //Delete Userdata
    public async Task<IActionResult> OnPostDeleteAsync()
    {
        var user = await LoadUserAsync();
        if (user == null) return Unauthorized();

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
        //Get relations such that follow links are wiped completely
        var author = await _db.Authors
            .Include(a => a.Cheeps)
            .Include(a => a.Followers)
            .Include(a => a.Following)
            .SingleOrDefaultAsync(a =>
                a.Username == user.UserName || (user.Email != null && a.Email == user.Email));

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


    //Helper method
    private async Task<ApplicationUser> LoadUserAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null) return user;

        var name = User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(name))
        {
            user = await _userManager.FindByNameAsync(name);
            if (user != null) return user;
        }

        var email = User.FindFirstValue(ClaimTypes.Email);
        if (!string.IsNullOrWhiteSpace(email))
        {
            user = await _userManager.FindByEmailAsync(email);
            if (user != null) return user;
        }

        return null;
    }
}
