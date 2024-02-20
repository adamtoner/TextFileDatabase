using System.Data;
using System.Diagnostics;
using System.Text;

namespace TextFileDatabase;

public class CsvObjectLoader<T> where T : new()
{
    private readonly string _filePath;
    private readonly bool _includeHeaders;
    private readonly char _fieldSeparator;

    public CsvObjectLoader(string filePath, bool includeHeaders = true, char fieldSeparator = ',')
    {
        _filePath = filePath;
        _includeHeaders = includeHeaders;
        _fieldSeparator = fieldSeparator;
        if (File.Exists(filePath))
            return;
        Save(Array.Empty<T>());
    }

    public void Save(IEnumerable<T> dataSet)
    {
        using var stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        using var writer = new StreamWriter(stream, Encoding.UTF8);
        var headers = typeof(T).GetProperties().Select(x => x.Name).ToArray();
        if (_includeHeaders)
            writer.WriteLine(string.Join(_fieldSeparator, headers));
        foreach (var item in dataSet)
        {
            var fields = new object[headers.Length];
            for (var i = 0; i < headers.Length; i++)
            {
                fields[i] = item.GetType()
                    .GetProperty(headers[i])
                    .GetValue(item);
            }

            writer.WriteLine(string.Join(_fieldSeparator, fields));
        }
    }

    public IEnumerable<T> Load()
    {
        using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);

        var headers = _includeHeaders
            ? reader.ReadLine().Split(_fieldSeparator)
            : typeof(T).GetProperties().Select(x => x.Name).ToArray();
        
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var fields = line.Split(_fieldSeparator);
            var result = new T();
            for (var i = 0; i < headers.Length; i++)
            {
                var value = fields[i];
                result.GetType().GetProperty(headers[i]).SetValue(result, value);
            }

            yield return result;
        }
    }
}