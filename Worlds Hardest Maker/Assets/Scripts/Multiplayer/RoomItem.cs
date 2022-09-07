using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
/// <summary>
/// controller of prefab RoomItem
/// </summary>
public class RoomItem : MonoBehaviour
{
    public RoomInfo info;
    public TMPro.TMP_Text roomNameTxt;
    public TMPro.TMP_Text playerCountTxt;

    private LobbyManager manager;

    private void Start()
    {
        // find global lobby manager script
        manager = FindObjectOfType<LobbyManager>();
    }

    public void SetRoomName(string _roomName)
    {
        if(roomNameTxt != null)
          roomNameTxt.text = _roomName;
    }

    public void SetPlayerCount(int playerCount)
    {
        if(playerCountTxt != null)
         playerCountTxt.text = $"{playerCount}/10";
    }

    /// <summary>
    /// onclick method for gameobject: join room with specific title
    /// </summary>
    public void OnClickItem()
    {
        PhotonNetwork.JoinRoom(roomNameTxt.text);
    }
}
