using Photon.Pun;
using UnityEngine;

/// <summary>
///     Controller for the clear level button
/// </summary>
public class ClearLevelController : MonoBehaviour
{
    public void OpenPrompt()
    {
        GetComponent<WarningConfirmPromptTween>().SetVisible(true);
    }

    public void ClosePrompt()
    {
        GetComponent<WarningConfirmPromptTween>().SetVisible(false);
    }

    public void ClearLevel()
    {
        ClosePrompt();

        if (MultiplayerManager.Instance.Multiplayer) GameManager.Instance.photonView.RPC("ClearLevel", RpcTarget.All);
        else GameManager.Instance.ClearLevel();
    }
}