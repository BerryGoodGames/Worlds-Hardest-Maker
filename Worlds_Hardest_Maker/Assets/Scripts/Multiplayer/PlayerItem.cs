using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    public TMP_Text playerName;

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
    }
}