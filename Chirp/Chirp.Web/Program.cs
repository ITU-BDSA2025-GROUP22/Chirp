using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Razor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Chirp.Razor.Areas.Identity;

var builder = WebApplication.CreateBuilder(args);

// "DefaultConnection" is a name referring to our database 'chirp.db' inside 'appsettings.json'
// This way we give EF Core context on our database schema
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbConn = new SqliteConnection(connectionString);
await dbConn.OpenAsync();

builder.Services.AddDbContext<ChirpContext>(options => options.UseSqlite(dbConn));
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ICheepService, CheepService>();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ChirpContext>();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
    
app.Run();

// Used for API testing
// A Program class had to exist in order for the tests to be able to reference it
public partial class Program { }