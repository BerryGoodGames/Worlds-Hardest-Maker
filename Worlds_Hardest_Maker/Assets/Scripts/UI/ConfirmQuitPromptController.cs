using MyBox;
using UnityEngine;

public class ConfirmQuitPromptController : WarningConfirmPromptController
{
    // TODO: weird dependency
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform fieldContainer;
    
    public override void OpenPrompt()
    {
        if (CheckOpenPrompt()) base.OpenPrompt();
        else OnConfirm();
    }
    
    private bool CheckOpenPrompt()
    {
        if (!LevelSessionManager.Instance.IsEdit) return false;
        
        if (PlayerManager.Instance.Player == null)
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