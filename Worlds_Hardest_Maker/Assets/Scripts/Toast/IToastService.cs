public interface IToastService
{
    public void ShowToast(ToastData toast);
    
    public void ShowToast(string message, float duration, ToastType type);
    public void ShowSuccess(string message, float duration) => ShowToast(message, duration, ToastType.Success);
    public void ShowInfo(string message, float duration) => ShowToast(message, duration, ToastType.Info);
    public void ShowWarning(string message, float duration) => ShowToast(message, duration, ToastType.Warning);
    public void ShowError(string message, float duration) => ShowToast(message, duration, ToastType.Error);
}