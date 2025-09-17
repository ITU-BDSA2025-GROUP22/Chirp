namespace Chirp.CLI.SimpleDB;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Program.Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            Console.WriteLine($"{cheep.Author}: {cheep.Message} \n({cheep.Timestamp})\n");
        }
    }

    public static void PrintCheeps(IEnumerable<Program.Cheep> cheeps, int num)
    {
        int count = 0;
        foreach (var cheep in cheeps)
        {
            Console.WriteLine($"{cheep.Author}: {cheep.Message} \n({cheep.Timestamp})\n");
            count++;
            if (count >= num)
                break;
        }
    }
}