using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    public TMP_Text PlayerName;

    public void SetPlayerInfo(Player player) => PlayerName.text = player.NickName;
}