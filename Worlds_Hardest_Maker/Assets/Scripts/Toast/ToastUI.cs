using MyBox;
using NUnit.Framework.Interfaces;
using TMPro;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

public class ToastUI : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_Text messageText;
    [SerializeField] [InitializationField] [MustBeAssigned] private Image iconImage;
    [SerializeField] [InitializationField] [MustBeAssigned] private Image lifetimeBar;
    
    private float totalLifetime;
    private float timeElapsed;
    
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
        Destroy(gameObject);
    }
}