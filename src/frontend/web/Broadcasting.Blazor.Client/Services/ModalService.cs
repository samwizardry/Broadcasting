namespace Broadcasting.Blazor.Client.Services;

public class ModalService : OpenableComponentService<int>
{
    public const string IsOpenCssClass = "is-open";

    public const string LoginModalCssClass = "login-modal";
    public const string RegisterModalCssClass = "register-modal";

    public const int LoginModal = 1;
    public const int RegisterModal = 2;
}
