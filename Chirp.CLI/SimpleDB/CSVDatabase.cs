using CsvHelper;
using System.Globalization;

namespace Chirp.CLI.SimpleDB;

public class CSVDatabase<T> : IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int? limit = null)
    {
        return Read(limit);
    }

    public void Store(T record)
    {
        
    }
}
