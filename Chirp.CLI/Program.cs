using System;
using System.IO;


public class Program
{
    public record Cheep(string Author, string Message, long Timestamp);

    public static void Main(string[] args)
    {
        string path = "data/chirp_cli_db.csv";
        DateTimeOffset localTime = DateTimeOffset.Now;
        DateTimeOffset utcTime = DateTimeOffset.UtcNow;

        if (args[0] == "read")
        {
            try
            {
                // Open the text file using a stream reader.
                using StreamReader reader = new(path);

                // Read the stream as a string.
                string text = reader.ReadToEnd();

                // Write the text to the console.
                Console.WriteLine(text);
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        if (args[0] == "cheep")
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(args [1]);
                sw.WriteLine("User: {0}", Environment.UserName);
                sw.WriteLine("Local Time:          {0}", localTime.ToString("T"));
                sw.WriteLine("Difference from UTC: {0}", localTime.Offset.ToString());
                sw.WriteLine("UTC:                 {0}", utcTime.ToString("T"));
            }
        }
    }
}

