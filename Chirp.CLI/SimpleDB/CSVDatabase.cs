using CsvHelper;
using System.Globalization;

namespace Chirp.SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private readonly string _path;
    public CSVDatabase(string path)
    {
        _path = path;
    }
    
    public IEnumerable<T> Read(int? limit = null)
    {
        using StreamReader reader = new(_path);
        using var csv =  new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<T>().ToList();
    }

    public void Store(T record)
    {
        using var writer = new StreamWriter(_path, append: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        csv.WriteRecord(record);
        csv.NextRecord();
    }
}
