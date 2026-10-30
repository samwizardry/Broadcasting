namespace Broadcasting.Blazor.Client.Services;

public abstract class OpenableComponentService<T>
    where T : IEquatable<T>
{
    private T _openedElement = default!;

    public event Action? OnChange;

    public bool IsOpen(T elementId) => elementId.Equals(_openedElement);

    public void Open(T elementId)
    {
        if (elementId.Equals(_openedElement))
            return;

        _openedElement = elementId;
        OnChange?.Invoke();
    }

    public void CloseAll()
    {
        if (_openedElement.Equals(default))
            return;

        _openedElement = default!;
        OnChange?.Invoke();
    }
}
