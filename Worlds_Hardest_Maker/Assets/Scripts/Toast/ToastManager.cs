using System;
using MyBox;
using UnityEngine;
using VContainer;

public class ToastManager : MonoBehaviour, IToastService
{
    [Inject] private ToastCanvas toastCanvas;
    
    [SerializeField] [InitializationField] [MustBeAssigned] private ToastTheme successTheme;
    [SerializeField] [InitializationField] [MustBeAssigned] private ToastTheme infoTheme;
    [SerializeField] [InitializationField] [MustBeAssigned] private ToastTheme warningTheme;
    [SerializeField] [InitializationField] [MustBeAssigned] private ToastTheme errorTheme;
    
    public void ShowToast(ToastData toast)
    {
        toastCanvas.InstantiateToast(toast);
    }
    
    public void ShowToast(string message, float duration, ToastType type)
    {
        ToastTheme theme = type switch
        {
            ToastType.Success => successTheme,
            ToastType.Info => infoTheme,
            ToastType.Warning => warningTheme,
            ToastType.Error => errorTheme,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
        
        ShowToast(new ToastData(message, duration, theme.Icon, theme.Color));
    }
}