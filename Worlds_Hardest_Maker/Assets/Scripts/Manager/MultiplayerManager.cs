using Photon.Pun;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    public bool Multiplayer { get; set; }


    public static void OnIsMultiplayer() =>
        // enable photon player spawning
        ReferenceManager.Instance.PlayerSpawner.enabled = true;

    private void Start()
    {
        if (Multiplayer) OnIsMultiplayer();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // check if multiplayer or not
        Instance.Multiplayer = PhotonNetwork.CurrentRoom != null;
    }
}