namespace Broadcasting.Streaming.Http;

internal record AddPathRequest(string Source,
    bool SourceOnDemand = true,
    string RunOnAvailable = "",
    string RunOnUnavailable = "",
    string RunOnOnline = "",
    string RunOnOffline = "");
