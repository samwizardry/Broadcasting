namespace Broadcasting.Blazor.Client.Services;

public class ModalService
{
    private string _currentModal = "";

    public const string LoginModalId = "login-modal";
    public const string RegisterModalId = "register-modal";

    public event Action? OnChange;

    public bool IsOpen(string modalId) => modalId == _currentModal;

    public void OpenModal(string modalId)
    {
        if (_currentModal == modalId)
            return;

        _currentModal = modalId;
        OnChange?.Invoke();
    }

    public void CloseAll()
    {
        if (string.IsNullOrEmpty(_currentModal))
            return;

        _currentModal = "";
        OnChange?.Invoke();
    }
}
