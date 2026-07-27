using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

public class ToastManager : MonoBehaviour, IToastService
{
    private EventBus eventBus;
    private ToastCanvas toastCanvas;
    
    [SerializeField] [InitializationField] [MustBeAssigned] private ToastTheme successTheme;
    [SerializeField] [InitializationField] [MustBeAssigned] private ToastTheme infoTheme;
    [SerializeField] [InitializationField] [MustBeAssigned] private ToastTheme warningTheme;
    [SerializeField] [InitializationField] [MustBeAssigned] private ToastTheme errorTheme;
    
    [Space] [SerializeField] [PositiveValueOnly] private int maxVisibleToasts = 3;
    
    private int currentToastCount;
    private readonly Queue<ToastData> overflowQueue = new();
    
    [Inject]
    private void Construct(EventBus eventBus, ToastCanvas toastCanvas)
    {
        this.eventBus = eventBus;
        this.toastCanvas = toastCanvas;
        
        eventBus.Subscribe<ToastPoppedEvent>(OnToastPopped);
    }
    
    private void OnToastPopped(ToastPoppedEvent evt)
    {
        // synchronize in case of multiple pops fired simultaneously
        lock (overflowQueue)
        {
            currentToastCount--;
        
            // enqueue overflowed toasts
            while (overflowQueue.Count > 0 && currentToastCount < maxVisibleToasts)
            {
                ToastData nextToast = overflowQueue.Dequeue();
                toastCanvas.InstantiateToast(nextToast);
                
                currentToastCount++;
            }
        }
    }
    
    public void ShowToast(ToastData toast)
    {
        lock (overflowQueue)
        {
            if (currentToastCount >= maxVisibleToasts)
            {
                overflowQueue.Enqueue(toast);
                return;
            }
            
            currentToastCount++;
        }
        
        // instantiate outside the lock to avoid blocking other threads
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
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<ToastPoppedEvent>(OnToastPopped);
    }
}