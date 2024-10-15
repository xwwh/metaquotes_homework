using MetaquotesHomework.Contracts;
using System.Reflection;

namespace MetaquotesHomework.Services;

public class GeobaseProvider: IGeobaseJsonProvider
{
    private Stack<IDisposable> _disposables = new Stack<IDisposable>();
    private readonly IConfiguration _configuration;
    private readonly HttpMessageHandler? _httpClientHandler;

    private Geobase _geobase = null!;
    private IGeobaseJson _geobaseJsonCache = null!;

    public GeobaseProvider(IConfiguration configuration, HttpMessageHandler? handler = null)
    {
        _configuration = configuration;
        _httpClientHandler = handler;
    }

    public void Init()
    {
        _geobase = GetGeobase();
        _geobaseJsonCache = new GeobaseJsonCache(_geobase);
    }

    public IGeobaseJson GetGeobaseJson()
    {
        if (_geobaseJsonCache == null)
            throw new Exception("Database not initialized");

        return _geobaseJsonCache;
    }

    public Geobase GetGeobase()
    {
        var section = _configuration.GetSection("Geobase");
        if (section == null)
            throw new NotImplementedException();
        var settings = section.Get<GeobaseSettings>();
        var stream = settings!.Provider switch
        {
            "file" => OpenFile(settings.Location),
            _ => throw new NotImplementedException()
        };
        _disposables.Push(stream);

        if (settings.Zipped)
        {
            stream = Unzip(stream, settings.ZipEntry);
            _disposables.Push(stream);
        }
        _geobase = GeobaseReader.Read(stream);
        Clear();
        return _geobase;
    }

    private static Stream OpenResource(string path)
    {
        var resource = Assembly.GetExecutingAssembly()
            .GetManifestResourceNames()
            .First(x => x.EndsWith(path));
        return Assembly.GetExecutingAssembly().GetManifestResourceStream(resource)!;
    }

    private static Stream OpenFile(string path)
    {
        return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    private Stream Unzip(Stream source, string? entry)
    {
        var zip = new System.IO.Compression.ZipArchive(source);
        _disposables.Push(zip);
        if(!string.IsNullOrEmpty(entry))
        {
            var item = zip.GetEntry(entry);
            return item!.Open();
        }
        else
        {
            var item = zip.Entries.First();
            return item!.Open();
        }
    }

    public void Dispose()
    {
        Clear();
    }

    private void Clear()
    {
        while (_disposables.Count > 0)
            _disposables.Pop().Dispose();
    }

    private record GeobaseSettings
    {
        public string Provider { get; set; } =  null!;
        public bool Zipped { get; set; }
        public string? ZipEntry { get; set; }
        public string Location { get; set; } = null!;
    }
}
