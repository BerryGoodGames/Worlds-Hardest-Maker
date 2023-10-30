using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

/// <summary>
///     Attach to prefab RoomItem
/// </summary>
public class RoomItem : MonoBehaviour
{
    public RoomInfo Info;
    public TMP_Text RoomNameTxt;

    public TMP_Text PlayerCountTxt;

    public void SetRoomName(string roomName)
    {
        if (RoomNameTxt != null) RoomNameTxt.text = roomName;
    }

    public void SetPlayerCount(int playerCount)
    {
        if (PlayerCountTxt != null) PlayerCountTxt.text = $"{playerCount}/10";
    }

    /// <summary>
    ///     OnClick method for gameObject: join room with specific title
    /// </summary>
    public void OnClickItem() => PhotonNetwork.JoinRoom(RoomNameTxt.text);
}