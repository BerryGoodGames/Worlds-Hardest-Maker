using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

[RequireComponent(typeof(ToastAnimation))]
public class ToastUI : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_Text messageText;
    [SerializeField] [InitializationField] [MustBeAssigned] private Image iconImage;
    [SerializeField] [InitializationField] [MustBeAssigned] private Image lifetimeBar;
    [SerializeField] [InitializationField] [MustBeAssigned] private ToastAnimation animation;
    
    private float totalLifetime;
    private float timeElapsed;
    
    [Inject] private EventBus eventBus;

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        
        lifetimeBar.fillAmount = 1f - timeElapsed / totalLifetime;
        
        if (timeElapsed > totalLifetime) Pop();
    }
    
    public void ApplyToastData(ToastData data)
    {
        messageText.text = data.Message;
        
        iconImage.sprite = data.Sprite;
        iconImage.color = data.Color;
        
        lifetimeBar.color = data.Color;
        
        totalLifetime = data.Duration;
        timeElapsed = 0;
    }
    
    private void Pop()
    { 
        animation.Death().OnComplete(() =>
        {
            eventBus.Fire(new ToastPoppedEvent());
            Destroy(gameObject);
        });
    }
}