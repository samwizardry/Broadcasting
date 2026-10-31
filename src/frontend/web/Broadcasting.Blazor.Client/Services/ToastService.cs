using Broadcasting.Blazor.Client.Components;

namespace Broadcasting.Blazor.Client.Services;

public class ToastService
{
    public class ToastMessage
    {
        public Guid Id { get; } = Guid.NewGuid();

        public AlertVariant Variant { get; set; } = AlertVariant.Info;

        public string? BadgeCode { get; set; }

        public string? Title { get; set; } = "TELEMETRY ALERT";

        public string? TelemetryCode { get; set; }

        public string? Message { get; set; }

        public bool IsFadingOut { get; set; }
    }

    public event Action<ToastMessage>? OnChange;

    public void AddMessage(ToastMessage message)
    {
        OnChange?.Invoke(message);
    }
}
