using UnityEngine;

public class ConfirmLeavePromptController : WarningConfirmPromptController
{
    public override void OpenPrompt()
    {
        if(CheckOpenPrompt()) base.OpenPrompt();
        else OnConfirm();
    }

    private bool CheckOpenPrompt()
    {
        if (PlayerManager.Instance.Player == null)
        {
            ConfirmationText.text = "Are you sure you want to quit?";
            WarningText.text = "The level does not contain a player!";
            return true;
        }
        
        if (!DoesGoalExist())
        {
            ConfirmationText.text = "Are you sure you want to quit?";
            WarningText.text = "The level does not contain a goal!";
            return true;
        }

        return false;
    }

    private static bool DoesGoalExist()
    {
        foreach (Transform field in ReferenceManager.Instance.FieldContainer)
        {
            if (field.CompareTag("Goal")) return true;
        }

        return false;
    }
}