namespace Broadcasting.Blazor.Client.Components;

public enum Size
{
    Normal,
    Sm,
    Lg
}

public enum ButtonVariant
{
    Primary,
    Secondary,
    Danger,
    Warning,
    Ghost
}

public enum AlertVariant
{
    Info,
    Success,
    Warning,
    Danger
}

public static class EnumExtensions
{
    public static string ToLowerString(this Size size)
        => size switch
        {
            Size.Normal => "",
            Size.Sm => "sm",
            Size.Lg => "lg",
            _ => throw new ArgumentException($"Size {size} is not supported.")
        };

    public static string ToLowerString(this ButtonVariant variant)
        => variant switch
        {
            ButtonVariant.Primary => "primary",
            ButtonVariant.Secondary => "secondary",
            ButtonVariant.Danger => "danger",
            ButtonVariant.Warning => "warning",
            ButtonVariant.Ghost => "ghost",
            _ => throw new ArgumentException($"Button variant {variant} is not supported.")
        };

    public static string ToLowerString(this AlertVariant type)
        => type switch
        {
            AlertVariant.Info => "info",
            AlertVariant.Success => "success",
            AlertVariant.Warning => "warning",
            AlertVariant.Danger => "danger",
            _ => throw new ArgumentException($"Alert type {type} is not supported.")
        };

    public static string GetBadgeCode(this AlertVariant type)
        => type switch
        {
            AlertVariant.Info => "SYS.INFO",
            AlertVariant.Success => "VERIFIED",
            AlertVariant.Warning => "WARN_08",
            AlertVariant.Danger => "CRIT_ERR",
            _ => throw new ArgumentException($"Alert type {type} is not supported.")
        };
}