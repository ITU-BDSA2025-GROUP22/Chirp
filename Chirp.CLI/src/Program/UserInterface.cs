namespace Chirp.CLI.SimpleDB;
// THIS CLASS IS NOT RELEVANT FOR WEB APP
// SHOULD LIKELY BE DELETED
public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            Console.WriteLine($"{cheep.Author}: {cheep.Message} \n({cheep.Timestamp})\n");
        }
    }
    //Static method for printing cheeps
    public static void PrintCheeps(IEnumerable<Cheep> cheeps, int num)
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