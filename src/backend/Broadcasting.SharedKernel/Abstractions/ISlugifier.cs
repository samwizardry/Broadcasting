using System.Diagnostics.CodeAnalysis;

namespace Broadcasting.SharedKernel.Abstractions;

public interface ISlugifier
{
    [return: NotNullIfNotNull(nameof(value))]
    public string? Slugify(string? value);
}
