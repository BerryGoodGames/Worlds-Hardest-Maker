using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

/// <summary>
///     controller of prefab RoomItem
/// </summary>
public class RoomItem : MonoBehaviour
{
    public RoomInfo info;
    public TMP_Text roomNameTxt;
    public TMP_Text playerCountTxt;

    public void SetRoomName(string _roomName)
    {
        if (roomNameTxt != null)
            roomNameTxt.text = _roomName;
    }

    public void SetPlayerCount(int playerCount)
    {
        if (playerCountTxt != null)
            playerCountTxt.text = $"{playerCount}/10";
    }

    /// <summary>
    ///     onclick method for gameObject: join room with specific title
    /// </summary>
    public void OnClickItem()
    {
        PhotonNetwork.JoinRoom(roomNameTxt.text);
    }
}