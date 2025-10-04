using Chirp.Razor;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<ICheepService, CheepService>();

var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
if (string.IsNullOrEmpty(dbPath))
{
    var tempDir = Path.GetTempPath();
    dbPath = Path.Combine(tempDir, "chirp.db");
}

builder.Services.AddDbContext<ChirpContext>(options =>
    options.UseSqlite($"Data Source={dbPath}")
);

var app = builder.Build();

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