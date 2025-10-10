using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Razor;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// "DefaultConnection" is a name referring to our database 'chirp.db' inside 'appsettings.json'
// This way we give EF Core context on our database schema
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbConn = new SqliteConnection(connectionString);
await dbConn.OpenAsync();

builder.Services.AddDbContext<ChirpContext>(options => options.UseSqlite(dbConn));
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepDBFacade, DBFacade>();
builder.Services.AddScoped<ICheepService, CheepService>();

var app = builder.Build();

// Add this block to automatically create/migrate the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChirpContext>();
    context.Database.EnsureCreated();
    
    // Seed data if database is empty
    if (!context.Cheeps.Any())
    {
        var author1 = new Author { Username = "Helge", Email = "helgenElgen@hotmail.dk", Password = "as5d4t44dsg45dge7g"};
        var author2 = new Author { Username = "Adrian", Email = "adri@gmail.dk", Password = "uf99v0tqvnt0gjdd03g" };
        
        context.Authors.AddRange(author1, author2);
        
        var cheep1 = new Cheep 
        { 
            Text = "Hello, BDSA students!", 
            TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Author = author1
        };
        
        var cheep2 = new Cheep 
        { 
            Text = "Welcome to Chirp!", 
            TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Author = author2
        };
        
        context.Cheeps.AddRange(cheep1, cheep2);
        context.SaveChanges();
    }
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

app.MapRazorPages();

app.Run();

// Used for API testing
// A Program class had to exist in order for the tests to be able to reference it
public partial class Program { }