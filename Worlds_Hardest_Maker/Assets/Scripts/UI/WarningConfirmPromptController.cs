using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WarningConfirmPromptTween))]
public class WarningConfirmPromptController : MonoBehaviour
{
    public TMP_Text ConfirmationText;
    public TMP_Text WarningText;
    
    [SerializeField] private UnityEvent confirm;
    
    protected WarningConfirmPromptTween Tween;
    private void Start() => Tween = GetComponent<WarningConfirmPromptTween>();

    public virtual void OpenPrompt() => Tween.SetVisible(true);

    public void ClosePrompt() => Tween.SetVisible(false);
    
    protected virtual void OnConfirm()
    {
        ClosePrompt();
        confirm.Invoke();
    }

    // method for unity inspector
    public void InvokeOnConfirm() => OnConfirm();
}