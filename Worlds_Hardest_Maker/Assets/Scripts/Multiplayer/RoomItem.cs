using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     controller of prefab RoomItem
/// </summary>
public class RoomItem : MonoBehaviour
{
    public RoomInfo Info;
    [FormerlySerializedAs("roomNameTxt")] public TMP_Text RoomNameTxt;

    [FormerlySerializedAs("playerCountTxt")]
    public TMP_Text PlayerCountTxt;

    public void SetRoomName(string roomName)
    {
        if (RoomNameTxt != null)
            RoomNameTxt.text = roomName;
    }

    public void SetPlayerCount(int playerCount)
    {
        if (PlayerCountTxt != null)
            PlayerCountTxt.text = $"{playerCount}/10";
    }

    /// <summary>
    ///     onclick method for gameObject: join room with specific title
    /// </summary>
    public void OnClickItem()
    {
        PhotonNetwork.JoinRoom(RoomNameTxt.text);
    }
}