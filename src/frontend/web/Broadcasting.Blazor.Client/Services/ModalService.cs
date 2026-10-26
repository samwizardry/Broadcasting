namespace Broadcasting.Blazor.Client.Services;

public class ModalService
{
    public const string LoginModalId = "login-modal";
    public const string RegisterModalId = "register-modal";

    public event Action? OnChange;

    private readonly Dictionary<string, bool> _modals = new()
    {
        { LoginModalId, false },
        { RegisterModalId, false }
    };

    public bool IsOpen(string modalId) => _modals[modalId];

    public void OpenModal(string modalId)
    {
        foreach (var id in _modals.Keys)
        {
            _modals[id] = false;
        }

        _modals[modalId] = true;
        OnChange?.Invoke();
    }

    public void CloseAll()
    {
        foreach (var modalId in _modals.Keys)
        {
            _modals[modalId] = false;
        }

        OnChange?.Invoke();
    }
}
