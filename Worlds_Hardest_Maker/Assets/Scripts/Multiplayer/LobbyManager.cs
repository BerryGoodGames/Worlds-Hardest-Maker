using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
///     inits lobby: rooms, players etc
/// </summary>
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager Instance { get; private set; }

    [FormerlySerializedAs("RoomItem")] public GameObject roomItem;
    [FormerlySerializedAs("PlayerItem")] public GameObject playerItem;
    [SerializeField] private LoadingScreen loadingScreen;

    [Space]

    // references
    public TMP_InputField roomNameInput;

    public GameObject lobbyPanel;
    public GameObject roomPanel;
    [SerializeField] private GameObject loadingPanel;
    public TMP_Text roomNameTitle;
    public TMP_Text yourName;
    [SerializeField] private string privateRoomStart = "!";
    [SerializeField] private Slider loadingSlider;

    [Space]

    // list of all room item controllers
    private readonly List<RoomItem> roomItemsList = new();

    public GameObject roomContainer;

    [Space]

    // vars for tracking cooldown
    public float timeBetweenUpdates = 1.5f;

    private float nextUpdateTime;

    [Space] private readonly List<PlayerItem> playerItemsList = new();
    public GameObject playerItemContainer;

    [Space] public GameObject playButton;

    private static readonly int load = Animator.StringToHash("Load");

    private void Start()
    {
        // start of lobby scene: join photon lobby
        PhotonNetwork.JoinLobby();

        yourName.text = $"Your Name: {PhotonNetwork.LocalPlayer.NickName}";
    }

    /// <summary>
    ///     onclick method for Create Room button
    /// </summary>
    public void OnClickCreate()
    {
        // if content in input
        if (roomNameInput.text.Length <= 0) return;

        // check if room already exists
        if (CheckRooms(roomNameInput.text))
        {
            // join room with name
            PhotonNetwork.JoinRoom(roomNameInput.text);
        }
        else
        {
            // create new room with name
            PhotonNetwork.CreateRoom(roomNameInput.text);
        }
    }

    /// <summary>
    ///     callback method: player joins / creates room: switch panels
    /// </summary>
    public override void OnJoinedRoom()
    {
        // hide lobby panel, show room panel with title
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        roomNameTitle.text = $"Room: {PhotonNetwork.CurrentRoom.Name}";

        UpdatePlayerList();
    }

    /// <summary>
    ///     callback method: global list of rooms updates in any way
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // check for cooldown
        if (!(Time.time >= nextUpdateTime)) return;

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
                GameObject newRoom = Instantiate(roomItem, roomContainer.transform);
                controller = newRoom.GetComponent<RoomItem>();
            }
            // create controller
            else controller = new();

            // set name and add to list
            controller.SetRoomName(room.Name);
            controller.SetPlayerCount(room.PlayerCount);
            controller.info = room;
            roomItemsList.Add(controller);
        }

        nextUpdateTime = Time.time + timeBetweenUpdates;
    }

    /// <summary>
    ///     onclick method for leave button
    /// </summary>
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    ///     callback method: player leaves any room: switch panels
    /// </summary>
    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    private void UpdatePlayerList()
    {
        // update player list
        // -> clear player item list
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }

        playerItemsList.Clear();

        // check if in a room
        if (PhotonNetwork.CurrentRoom == null) return;

        // fill in all connected player
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            // init new player item
            GameObject newPlayerItem = Instantiate(playerItem, playerItemContainer.transform);

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
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }

        // check enter key
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClickCreate();
        }
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
    ///     checks if room already exists
    /// </summary>
    /// <param name="roomName">name of room to check</param>
    public bool CheckRooms(string roomName)
    {
        foreach (RoomItem room in roomItemsList)
        {
            if (room.info.Name.Equals(roomName))
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