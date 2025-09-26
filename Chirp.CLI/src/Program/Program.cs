using System;
using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Chirp.CLI.SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string path = "../../data/chirp_cli_db.csv";
var db = new CSVDatabase<Cheep>(path);

app.MapGet("/cheeps", () => db.Read());
app.MapPost("/cheep", (Cheep cheep) =>
{
    Console.WriteLine($"Received: {cheep.Author}, {cheep.Message}, {cheep.Timestamp}");
    db.Store(cheep);
    return Results.Ok();
});

app.Run();

public record Cheep(string Author, string Message, string Timestamp);

// kør med
// dotnet run
// åben ny terminal
// kør så
/*
Invoke-RestMethod -Uri "http://localhost:5000/cheep" `
    >>   -Method Post `
    >>   -ContentType "application/json" `
    >>   -Body '{"author":"ropf","message":"Hello from PowerShell!","timestamp":"1684229348"}  
*/