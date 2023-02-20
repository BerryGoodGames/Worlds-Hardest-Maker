using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerItem : MonoBehaviour
{
    [FormerlySerializedAs("playerName")] public TMP_Text PlayerName;

    public void SetPlayerInfo(Player player)
    {
        PlayerName.text = player.NickName;
    }
}