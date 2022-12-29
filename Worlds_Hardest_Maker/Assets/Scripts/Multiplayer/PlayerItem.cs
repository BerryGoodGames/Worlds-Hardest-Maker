using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    public TMP_Text playerName;

    public void SetPlayerInfo(Player player)
    {
        playerName.text = player.NickName;
    }
}