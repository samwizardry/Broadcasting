using System.Diagnostics.CodeAnalysis;

using Broadcasting.SharedKernel.Abstractions;

namespace Broadcasting.SharedKernel.Services;

public class DefaultSlugifier : ISlugifier
{
    [return: NotNullIfNotNull(nameof(text))]
    public string? Slugify(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text?.Trim();
        }

        int len = 0;
        Span<char> slug = stackalloc char[text.Length];

        foreach (var c in text)
        {
            char currentChar = char.ToLowerInvariant(c);
            if (currentChar >= 'a' && currentChar <= 'z')
            {
                slug[len++] = currentChar;
            }

        }

        return slug.Slice(0, len).ToString();
    }
}
