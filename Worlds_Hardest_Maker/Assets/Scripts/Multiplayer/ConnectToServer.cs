using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private LoadingScreen loadingScreen;

    [FormerlySerializedAs("usernameInput")]
    public TMP_InputField UsernameInput;

    [FormerlySerializedAs("buttonText")] public TMP_Text ButtonText;

    public void OnClickConnect()
    {
        if (UsernameInput.text.Length <= 0) return;

        PhotonNetwork.NickName = UsernameInput.text;
        ButtonText.text = "Connecting...";

        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void MainMenu()
    {
        loadingScreen.LoadScene(0);
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