using System.Collections.ObjectModel;

namespace Broadcasting.ServiceDefaults;

public static class KnownServices
{
    public const string Backend = "backend";
    public const string Frontend = "frontend";
    public const string MediaMTX = "mediamtx";

    public static readonly ReadOnlyDictionary<string, string[]> DefaultEndpoints = new Dictionary<string, string[]>()
    {
        { Backend, ["https://localhost:7167", "http://localhost:5246"] },
        { Frontend, ["https://localhost:7135", "http://localhost:5170"] },
        { MediaMTX, ["http://localhost:9997"] }
    }.AsReadOnly();
}
