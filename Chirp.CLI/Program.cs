using System;
using System.IO;

using Chirp.CLI.SimpleDB;


public class Program
{
    public record Cheep(string Author, string Message, string Timestamp);

    public static void Main(string[] args)
    {
        string path = "data/chirp_cli_db.csv";

        var db = new CSVDatabase<Cheep>(path);

        if (args[0] == "read")
        {
            foreach (var cheep in db.Read())
            {
                Console.WriteLine($"{cheep.Author}: {cheep.Message} \n({cheep.Timestamp})\n");
            }
        }

        if (args[0] == "cheep")
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