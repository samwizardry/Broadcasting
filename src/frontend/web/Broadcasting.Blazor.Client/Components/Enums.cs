using System.Drawing;

namespace Broadcasting.Blazor.Client.Components;

public enum Size
{
    None, // Как md
    Sm,
    Md,
    Lg,
    Xl
}

public enum CyberAccent
{
    Acid,
    Cyan,
    Alert,
    Warning,
    Purple
}

public enum SurfaceTier
{
    Deep, // подложка страницы
    Surface, // стандартный блок/карточка
    Elevated //модалки, поповеры, меню
}

// Buttons

public enum ButtonVariant
{
    Primary,
    Secondary,
    Danger,
    Warning,
    Ghost
}

public enum ButtonType
{
    Button,
    Submit,
    Reset
}

// Alerts

public enum AlertVariant
{
    Info,
    Success,
    Warning,
    Danger
}

// Forms

public enum InputType
{
    Text,
    Password
}

public enum FieldState
{
    Normal,
    Success,
    Danger,
    Error,
    Disabled,
    Readonly
}

public static class EnumExtensions
{
    public static string ToLowerString(this CyberAccent accent) => accent switch
    {
        CyberAccent.Acid => "acid",
        CyberAccent.Cyan => "cyan",
        CyberAccent.Alert => "alert",
        CyberAccent.Warning => "warning",
        CyberAccent.Purple => "purple",
        _ => throw new ArgumentException($"Accent {accent} is not supported.")
    };

    public static string ToLowerString(this Size size) => size switch
    {
        Size.Sm => "sm",
        Size.Md => "md",
        Size.Lg => "lg",
        Size.Xl => "xl",
        _ => throw new ArgumentException($"Size {size} is not supported.")
    };

    // Buttons

    public static string ToLowerString(this ButtonVariant variant) => variant switch
    {
        ButtonVariant.Primary => "primary",
        ButtonVariant.Secondary => "secondary",
        ButtonVariant.Danger => "danger",
        ButtonVariant.Warning => "warning",
        ButtonVariant.Ghost => "ghost",
        _ => throw new ArgumentException($"Button variant {variant} is not supported.")
    };

    public static string ToLowerString(this ButtonType type) => type switch
    {
        ButtonType.Button => "button",
        ButtonType.Submit => "submit",
        ButtonType.Reset => "reset",
        _ => throw new ArgumentException($"Button type {type} is not supported.")
    };

     // Alerts

    public static string ToLowerString(this AlertVariant variant) => variant switch
    {
        AlertVariant.Info => "info",
        AlertVariant.Success => "success",
        AlertVariant.Warning => "warning",
        AlertVariant.Danger => "danger",
        _ => throw new ArgumentException($"Alert variant {variant} is not supported.")
    };

    public static string ToCode(this AlertVariant variant) => variant switch
    {
        AlertVariant.Info => "SYS.INFO",
        AlertVariant.Success => "VERIFIED",
        AlertVariant.Warning => "WARN_08",
        AlertVariant.Danger => "CRIT_ERR",
        _ => throw new ArgumentException($"Alert variant {variant} is not supported.")
    };

    // Forms

    public static string ToLowerCase(this InputType type) => type switch
    {
        InputType.Text => "text",
        InputType.Password => "password",
        _ => throw new ArgumentException($"Input type {type} is not supported.")
    };

    public static string ToLowerCase(this FieldState state) => state switch
    {
        FieldState.Success => "success",
        FieldState.Danger => "danger",
        FieldState.Error => "error",
        FieldState.Disabled => "disabled",
        FieldState.Readonly => "readonly",
        _ => throw new ArgumentException($"State {state} is not supported.")
    };

    public static string ToCode(this FieldState state) => state switch
    {
        FieldState.Success => "VERIFIED",
        FieldState.Danger => "WARN_08",
        FieldState.Error => "CRIT_ERR",
        FieldState.Disabled => throw new NotImplementedException(),
        FieldState.Readonly => throw new NotImplementedException(),
        _ => throw new ArgumentException($"State {state} is not supported.")
    };
}