using System;
using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;

using Chirp.CLI.SimpleDB;

namespace Chirp.CLI;
public class Program
{
    public record Cheep(string Author, string Message, string Timestamp);

    public static void Main(string[] args)
    {
        var cheepOption = new Option<string>(
            name: "cheep"
        );
        var readOption = new Option<bool>(
            name: "read"
        )
        {
            Arity = ArgumentArity.Zero // <-- This makes it a switch/flag
        };

        var rootCommand = new RootCommand("userArgs");
        rootCommand.Add(readOption);
        rootCommand.Add(cheepOption);
        
        string path = "../../data/chirp_cli_db.csv";

        var db = new CSVDatabase<Cheep>(path);
        
        ParseResult parseResult = rootCommand.Parse(args);
        var readValue = parseResult.GetValue(readOption);
        var cheepValue = parseResult.GetValue(cheepOption);
        //reads all cheeps
        if (readValue)
        {
            UserInterface.PrintCheeps(db.Read());
        }
        //stores cheep in database
        if (cheepValue is not null)
        {
            var cheep = new Cheep(
                Environment.UserName,
                args[1],
                DateTimeOffset.Now.ToLocalTime().ToString()
            );
            db.Store(cheep);
        }
    }
}