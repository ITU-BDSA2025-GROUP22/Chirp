using System;
using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Chirp.SimpleDB;

// CLI CLIENT
namespace Chirp.CLI
{
    public class Program
    {
        public record Cheep(string Author, string Message, string Timestamp);

        public static async Task Main(string[] args)
        {
            //var baseURL = "http://localhost:5049";
            var baseURL = "https://bdsagroup22chirpremotedb.azurewebsites.net";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseURL);
            
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
            
            var parseResult = rootCommand.Parse(args);
            var readValue = parseResult.GetValue(readOption);
            var cheepValue = parseResult.GetValue(cheepOption);

            if (readValue)
            {
                var cheeps = await client.GetFromJsonAsync<IEnumerable<Cheep>>("cheeps");
                if (cheeps is not null)
                {
                    UserInterface.PrintCheeps(cheeps);
                }
                else
                {
                    Console.WriteLine("Could not find any cheeps");
                }
            }

            if (cheepValue != null)
            {
                var cheep = new Cheep(
                    Environment.UserName,
                    cheepValue,
                    DateTimeOffset.Now.ToLocalTime().ToString()
                );
                var response = await client.PostAsJsonAsync("cheep", cheep);
                if (response.IsSuccessStatusCode)
                    Console.WriteLine("Cheep posted successfully!");
                else
                    Console.WriteLine($"Failed to post cheep. Status code: {response.StatusCode}");
            }
        }
    }
}