using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public TMPro.TMP_InputField usernameInput;
    public TMPro.TMP_Text buttonText;

    public void OnClickConnect()
    {
        if(usernameInput.text.Length > 0)
        {
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Connecting...";

            PhotonNetwork.AutomaticallySyncScene = true;

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }

    private void Update()
    {
        // check for enter key
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClickConnect();
        }
    }
}
