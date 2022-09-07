using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controller for the clear level button
/// </summary>

public class ClearLevelController : MonoBehaviour
{
    [SerializeField] private GameObject prompt;

    public void OpenPrompt()
    {
        prompt.GetComponent<Animator>().SetBool("Active", true);
    }

    public void ClosePrompt()
    {
        prompt.GetComponent<Animator>().SetBool("Active", false);
    }

    public void ClearLevel()
    {
        ClosePrompt();

        if (GameManager.Instance.Multiplayer) GameManager.Instance.photonView.RPC("ClearLeve", Photon.Pun.RpcTarget.All);
        else GameManager.Instance.ClearLevel();
    }
}
