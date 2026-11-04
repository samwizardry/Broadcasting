namespace Broadcasting.Contracts.Streaming;

public class StreamChannelResponse
{
    public string Username { get; set; } = string.Empty;

    public string ChannelSlug { get; set; } = string.Empty;

    public string HlsAddress { get; set; } = string.Empty;

    public bool IsLive { get; set; }
}
