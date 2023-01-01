using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInput;
    public TMP_Text buttonText;

    public void OnClickConnect()
    {
        if (usernameInput.text.Length <= 0) return;

        PhotonNetwork.NickName = usernameInput.text;
        buttonText.text = "Connecting...";

        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
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