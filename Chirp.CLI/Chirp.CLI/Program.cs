using System;
using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;

// CLI CLIENTx
using Chirp.SimpleDB;

namespace Chirp.CLI
{
    public class Program
    {
        public record Cheep(string Author, string Message, string Timestamp);

        public static void Main(string[] args)
        {
            var baseURL = "http://localhost:5012";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseURL);
            
            // Send an asynchronous HTTP GET request and automatically construct a Cheep object from the
            // JSON object in the body of the response
            var cheeps = await client.GetFromJsonAsync<Cheep>("cheeps");
            
            string path = "../data/chirp_cli_db.csv";
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
            ParseResult parseResult = rootCommand.Parse(args);
            
            await client.PostAsJsonAsync("cheep", cheep);

            /*
             ****OLD PARSING****

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
            }*/
        }
    }
}