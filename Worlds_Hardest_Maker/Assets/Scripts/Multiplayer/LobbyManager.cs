using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
///     Initializes lobby: rooms, players etc
/// </summary>
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager Instance { get; private set; }

    [FormerlySerializedAs("roomItem")] public GameObject RoomItem;
    [FormerlySerializedAs("playerItem")] public GameObject PlayerItem;
    [SerializeField] private LoadingScreen loadingScreen;

    [FormerlySerializedAs("roomNameInput")] [Space]

    // references
    public TMP_InputField RoomNameInput;

    [FormerlySerializedAs("lobbyPanel")] public GameObject LobbyPanel;
    [FormerlySerializedAs("roomPanel")] public GameObject RoomPanel;
    [SerializeField] private GameObject loadingPanel;

    [FormerlySerializedAs("roomNameTitle")]
    public TMP_Text RoomNameTitle;

    [FormerlySerializedAs("yourName")] public TMP_Text YourName;
    [SerializeField] private string privateRoomStart = "!";
    [SerializeField] private Slider loadingSlider;

    [Space]

    // list of all room item controllers
    private readonly List<RoomItem> roomItemsList = new();

    [FormerlySerializedAs("roomContainer")]
    public GameObject RoomContainer;

    [FormerlySerializedAs("timeBetweenUpdates")] [Space]

    // vars for tracking cooldown
    public float TimeBetweenUpdates = 1.5f;

    private float nextUpdateTime;

    [Space] private readonly List<PlayerItem> playerItemsList = new();

    [FormerlySerializedAs("playerItemContainer")]
    public GameObject PlayerItemContainer;

    [FormerlySerializedAs("playButton")] [Space]
    public GameObject PlayButton;

    private static readonly int load = Animator.StringToHash("Load");

    private void Start()
    {
        // start of lobby scene: join photon lobby
        PhotonNetwork.JoinLobby();

        YourName.text = $"Your Name: {PhotonNetwork.LocalPlayer.NickName}";
    }

    /// <summary>
    ///     OnClick callback (for Create Room button)
    /// </summary>
    public void OnClickCreate()
    {
        // if content in input
        if (RoomNameInput.text.Length <= 0) return;

        // check if room already exists
        if (CheckRooms(RoomNameInput.text))
            // join room with name
            PhotonNetwork.JoinRoom(RoomNameInput.text);
        else
            // create new room with name
            PhotonNetwork.CreateRoom(RoomNameInput.text);
    }

    /// <summary>
    ///     OnJoin / OnCreate callback
    /// </summary>
    public override void OnJoinedRoom()
    {
        // hide lobby panel, show room panel with title
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(true);

        RoomNameTitle.text = $"Room: {PhotonNetwork.CurrentRoom.Name}";

        UpdatePlayerList();
    }

    /// <summary>
    ///     Callback method: global list of rooms updates in any way
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // check for cooldown
        if (Time.time < nextUpdateTime) return;

        // update room list
        // -> clear list
        foreach (RoomItem item in roomItemsList)
        {
            if (item != null && item.gameObject != null)
                Destroy(item.gameObject);
        }

        roomItemsList.Clear();

        // fill list with new RoomItem game objects
        foreach (RoomInfo room in roomList)
        {
            RoomItem controller;
            if (!room.Name.StartsWith(privateRoomStart))
            {
                // new RoomItem in roomContainer
                GameObject newRoom = Instantiate(RoomItem, RoomContainer.transform);
                controller = newRoom.GetComponent<RoomItem>();
            }
            // create controller
            else
            {
                controller = new();
            }

            // set name and add to list
            controller.SetRoomName(room.Name);
            controller.SetPlayerCount(room.PlayerCount);
            controller.Info = room;
            roomItemsList.Add(controller);
        }

        nextUpdateTime = Time.time + TimeBetweenUpdates;
    }

    /// <summary>
    ///     OnClick method for leave button
    /// </summary>
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    ///     Callback method: player leaves any room: switch panels
    /// </summary>
    public override void OnLeftRoom()
    {
        RoomPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    private void UpdatePlayerList()
    {
        // update player list
        // -> clear player item list
        foreach (PlayerItem item in playerItemsList) Destroy(item.gameObject);

        playerItemsList.Clear();

        // check if in a room
        if (PhotonNetwork.CurrentRoom == null) return;

        // fill in all connected player
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            // init new player item
            GameObject newPlayerItem = Instantiate(PlayerItem, PlayerItemContainer.transform);

            // add to list
            PlayerItem controller = newPlayerItem.GetComponent<PlayerItem>();
            controller.SetPlayerInfo(player.Value);
            playerItemsList.Add(controller);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    private void Update()
    {
        // only show play button to host
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 1)
            PlayButton.SetActive(true);
        else
            PlayButton.SetActive(false);

        // check enter key
        if (Input.GetKeyDown(KeyCode.Return)) OnClickCreate();
    }

    public void OnClickPlayButton()
    {
        photonView.RPC("StartLoading", RpcTarget.All);

        PhotonNetwork.LoadLevel("DefaultEditorScene");
    }

    [PunRPC]
    private void StartLoading()
    {
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.SetProgress(0);

        // Animator anim = loadingPanel.GetComponent<Animator>();
        // anim.SetTrigger(load);

        StartCoroutine(UpdateLoadingSlider());
    }

    private IEnumerator UpdateLoadingSlider()
    {
        while (PhotonNetwork.LevelLoadingProgress <= 0.9)
        {
            loadingScreen.SetProgress(PhotonNetwork.LevelLoadingProgress / 0.9f);
            yield return null;
        }
    }

    /// <summary>
    ///     Checks if room already exists
    /// </summary>
    public bool CheckRooms(string roomName)
    {
        foreach (RoomItem room in roomItemsList)
        {
            if (room.Info.Name.Equals(roomName))
                return true;
        }

        return false;
    }

    public void Back()
    {
        loadingScreen.LoadScene(2);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}