using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Broadcasting.Contracts.Responses;

/// <summary>
/// RFC9457 compatible problem details/ error response class. this can be used by configuring startup like so:
/// <code>app.UseFastEndpoints(c => c.Errors.UseProblemDetails())</code>
/// </summary>
public sealed class ProblemDetails
{
    public const string DefaultTitleValue = "One or more validation errors occurred.";

    [DefaultValue("https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1")]
    public string Type { get; set; } = null!;

    [DefaultValue(DefaultTitleValue)]
    public string Title { get; set; } = null!;

    [DefaultValue(400)]
    public int Status { get; set; }

    [DefaultValue("/api/route")]
    public string Instance { get; set; } = null!;

    [DefaultValue("0HMPNHL0JHL76:00000001")]
    public string TraceId { get; set; } = null!;

    /// <summary>
    /// the details of the error
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Detail { get; set; }

    public IReadOnlyCollection<Error> Errors { get; set; } = null!;

    /// <summary>
    /// the error details object
    /// </summary>
    public sealed class Error
    {
        /// <summary>
        /// the name of the error or property of the dto that caused the error
        /// </summary>
        [DefaultValue("Error or field name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// the reason for the error
        /// </summary>
        [DefaultValue("Error reason")]
        public string Reason { get; set; } = null!;

        /// <summary>
        /// the code of the error
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Code { get; set; }

        /// <summary>
        /// the severity of the error
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Severity { get; set; }
    }
}
