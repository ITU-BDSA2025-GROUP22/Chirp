using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Chirp.Web.Areas.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNet.Security.OAuth.GitHub;

var builder = WebApplication.CreateBuilder(args);

// "DefaultConnection" is a name referring to our database 'chirp.db' inside 'appsettings.json'
// This way we give EF Core context on our database schema
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbConn = new SqliteConnection(connectionString);
await dbConn.OpenAsync();

builder.Services
    .AddDbContext<ChirpContext>(options => options.UseSqlite(dbConn))
    .AddScoped<ICheepRepository, CheepRepository>()
    .AddScoped<IAuthorRepository, AuthorRepository>()
    .AddScoped<ICheepService, CheepService>()
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ChirpContext>();

builder.Services
    .AddAuthentication(options =>
    {
        // Default scheme for cookies (used by Identity)
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddCookie()
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
        options.CallbackPath = "/signin-github";
    });
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Add this block to automatically create/migrate the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChirpContext>();
    context.Database.EnsureCreated();
    
    // Seed data if database is empty
    DbInitializer.SeedDatabase(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

// Used for API testing
// A Program class had to exist in order for the tests to be able to reference it