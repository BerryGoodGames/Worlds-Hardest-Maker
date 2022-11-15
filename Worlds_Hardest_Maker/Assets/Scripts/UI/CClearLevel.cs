using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Controller for the clear level button
/// </summary>
public class CClearLevel : MonoBehaviour
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

        if (MGame.Instance.Multiplayer) MGame.Instance.photonView.RPC("ClearLevel", RpcTarget.All);
        else MGame.Instance.ClearLevel();
    }
}
