namespace Broadcasting.Contracts.Streaming;

public class StreamChannelResponse
{
    public string Username { get; set; } = string.Empty;
    public string RtspUrl { get; set; } = string.Empty;
    public bool IsLive { get; set; }
}
