namespace TextFileDatabase;

public class ObjectCache<T> where T : new()
{
    private readonly CsvObjectLoader<T> _loader;
    private readonly TimeSpan _reloadPeriod;
    private DateTime _lastReloaded;
    
    public ObjectCache(string filePath, TimeSpan reloadPeriod, char fieldSeparator = ',')
    {
        _loader = new CsvObjectLoader<T>(filePath, true, fieldSeparator);
        _reloadPeriod = reloadPeriod;
        Refresh();
    }

    private IEnumerable<T> _cache;

    public IEnumerable<T> Cache
    {
        get
        {
            if (_lastReloaded + _reloadPeriod < DateTime.UtcNow)
                Refresh();
            return _cache;
        }
        set
        {
            _cache = value;
            _loader.Save(_cache);
        }
    }

    public void Refresh()
    {
        _cache = _loader.Load();
        _lastReloaded = DateTime.UtcNow;
    }
}