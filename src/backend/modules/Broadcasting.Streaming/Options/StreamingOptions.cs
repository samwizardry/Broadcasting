using System.ComponentModel.DataAnnotations;

namespace Broadcasting.Streaming.Options;

public class StreamingOptions
{
    public const string Key = "Modules:Streaming";

    [Required]
    public string RtspAddress { get; set; } = default!;

    [Required]
    public string HlsAddress { get; set; } = default!;
}
