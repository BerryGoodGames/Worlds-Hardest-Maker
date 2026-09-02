using MyBox;
using UnityEngine;
using VContainer;

public class ConfirmQuitPromptController : WarningConfirmPromptController
{
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform fieldContainer;
    
    // TODO: check if injected
    [Inject] private IPlayerProvider playerProvider;
    
    public override void OpenPrompt()
    {
        if (CheckOpenPrompt()) base.OpenPrompt();
        else OnConfirm();
    }
    
    private bool CheckOpenPrompt()
    {
        if (!LevelSessionManager.Instance.IsEdit) return false;
        
        if (!playerProvider.HasPlayer)
        {
            ConfirmationText.text = "Are you sure you want to quit?";
            WarningText.text = "The level does not contain a player!";
            return true;
        }
        
        if (DoesGoalExist()) return false;
        
        ConfirmationText.text = "Are you sure you want to quit?";
        WarningText.text = "The level does not contain a goal!";
        return true;
    }
    
    private bool DoesGoalExist()
    {
        foreach (Transform field in fieldContainer)
        {
            if (field.CompareTag("Goal")) return true;
        }
        
        return false;
    }
}