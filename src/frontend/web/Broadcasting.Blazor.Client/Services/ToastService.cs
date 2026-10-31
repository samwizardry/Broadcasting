using System.Text.Json;

using Broadcasting.Blazor.Client.Components;

using Microsoft.JSInterop;

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

    private const string StorageKey = "broadcasting.pending_toast";
    private readonly IJSRuntime _jsRuntime;

    public event Action<ToastMessage>? OnChange;

    public ToastService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void AddMessage(ToastMessage message)
    {
        OnChange?.Invoke(message);
    }

    public ValueTask AddMessageAfterReloadAsync(ToastMessage message)
    {
        var json = JsonSerializer.Serialize(message);
        return _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", StorageKey, json);
    }

    public async ValueTask ShowPendingMessagesAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", StorageKey);

        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", StorageKey);

        var message = JsonSerializer.Deserialize<ToastMessage>(json);
        if (message is not null)
        {
            AddMessage(message);
        }
    }
}
