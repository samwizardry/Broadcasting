using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Broadcasting.SharedKernel.Http;

/// <summary>
/// RFC 9457 compatible problem details/error response class.
/// </summary>
public sealed class ProblemDetails : IError<ProblemDetails>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }

    public string Title { get; set; } = string.Empty;

    public int Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Instance { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Detail { get; set; }

    public IReadOnlyCollection<Error> Errors { get; set; } = [];

    public static ProblemDetails Create(int statusCode, string title, string detail) =>
        new()
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

    public static Task<ProblemDetails?> CreateAsync(HttpResponseMessage response, CancellationToken ct = default) =>
        response.Content.ReadFromJsonAsync<ProblemDetails>(ct);

    public sealed class Error
    {
        public string Name { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Code { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Severity { get; set; }
    }
}
