using UnityEngine;

/// <summary>
///     Controller for the clear level button
/// </summary>
public class ClearLevelController : MonoBehaviour
{
    public void OpenPrompt() => GetComponent<WarningConfirmPromptTween>().SetVisible(true);

    public void ClosePrompt() => GetComponent<WarningConfirmPromptTween>().SetVisible(false);

    public void ClearLevel()
    {
        ClosePrompt();
        GameManager.Instance.ClearLevel();
    }
}