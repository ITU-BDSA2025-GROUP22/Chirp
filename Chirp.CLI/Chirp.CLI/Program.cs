using System;
using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
using Chirp.CLI.SimpleDB;

namespace Chirp.CLI
{
    public class Program
    {
        public record Cheep(string Author, string Message, string Timestamp);

        public static void Main(string[] args)
        {
            string path = "../../data/chirp_cli_db.csv";
            var db = new CSVDatabase<Cheep>(path);

            // Define CLI options
            var cheepOption = new Option<string>(
                name: "cheep"
            );

            var readOption = new Option<bool>(
                name: "read"
            )
            {
                Arity = ArgumentArity.Zero // makes this a switch/flag
            };

            // Set up the root command
            var rootCommand = new RootCommand("Chirp CLI");
            rootCommand.Add(readOption);
            rootCommand.Add(cheepOption);

            // Parse args
            ParseResult parseResult = rootCommand.Parse(args);
            var readValue = parseResult.GetValue(readOption);
            var cheepValue = parseResult.GetValue(cheepOption);

            // Read all cheeps
            if (readValue)
            {
                UserInterface.PrintCheeps(db.Read());
            }

            // Store a new cheep
            if (cheepValue is not null)
            {
                var cheep = new Cheep(
                    Environment.UserName,
                    cheepValue,
                    DateTimeOffset.Now.ToLocalTime().ToString()
                );

                db.Store(cheep);
            }
        }
    }
}