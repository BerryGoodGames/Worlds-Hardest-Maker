using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    public TMPro.TMP_Text playerName;

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
    }
}
