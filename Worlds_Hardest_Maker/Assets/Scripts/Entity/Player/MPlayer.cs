using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// manages player duh
/// </summary>
public class MPlayer : MonoBehaviour
{
    public static MPlayer Instance { get; private set; }

    // list of fields which are safe for player
    public static readonly List<MField.FieldType> SafeFields = new(new MField.FieldType[]{
        
    });
    public static readonly List<MField.FieldType> StartFields = new(new MField.FieldType[]
    {
        MField.FieldType.START_FIELD,
        MField.FieldType.GOAL_FIELD
    });

    public void SetPlayer(float mx, float my, float speed)
    {
        if (CanPlace(mx, my))
        {
            // clear area from coins and keys
            MGame.RemoveObjectInContainer(mx, my, MGame.Instance.CoinContainer);
            MGame.RemoveObjectInContainer(mx, my, MGame.Instance.KeyContainer);

            // clear all players (only from this client tho)
            if (MGame.Instance.Multiplayer)
            {
                CPlayer[] players = FindObjectsOfType<CPlayer>();
                foreach (CPlayer p in players)
                {
                    PhotonView view = p.GetComponent<PhotonView>();
                    // check if player is from own client
                    if (view.IsMine)
                    {
                        // remove player
                        MGame.Instance.photonView.RPC("RemovePlayerAtPosOnlyOtherClients", RpcTarget.Others, p.transform.position.x, p.transform.position.y);
                        RemovePlayerAtPosIgnoreOtherClients(p.transform.position.x, p.transform.position.y);
                    }
                }
            }
            else
            {
                RemoveAllPlayers();
            }

            // place player
            GameObject newPlayer = InstantiatePlayer(mx, my, speed, MGame.Instance.Multiplayer);
            
            int newID = AvailableID();
            newPlayer.GetComponent<CPlayer>().id = newID;

            // set target of camera
            Camera.main.GetComponent<JumpToEntity>().target = newPlayer;
        }
    }
    [PunRPC]
    public void SetPlayer(float mx, float my)
    {
        SetPlayer(mx, my, 3f);
    }

    [PunRPC]
    public void RemoveAllPlayers()
    {
        foreach(Transform player in MGame.Instance.PlayerContainer.transform)
        {
            player.GetComponent<CPlayer>().DestroyPlayer();
        }
    }
    [PunRPC]
    public void RemovePlayerAtPos(float mx, float my)
    {
        // remove player only if at pos
        foreach (Transform player in MGame.Instance.PlayerContainer.transform)
        {
            if(player.position.x == mx && player.position.y == my)
            {
                player.GetComponent<CPlayer>().DestroyPlayer();
            }
        }
    }
    [PunRPC]
    public void RemovePlayerAtPosOnlyOtherClients(float mx, float my)
    {
        // remove player only if at pos
        foreach (Transform player in MGame.Instance.PlayerContainer.transform)
        {
            if (MGame.Instance.Multiplayer && player.GetComponent<PhotonView>().IsMine) continue;
            if (player.position.x == mx && player.position.y == my)
            {
                player.GetComponent<CPlayer>().DestroyPlayer();
            }
        }
    }
    [PunRPC]
    public void RemovePlayerAtPosIgnoreOtherClients(float mx, float my)
    {
        // remove player only if at pos
        foreach (Transform player in MGame.Instance.PlayerContainer.transform)
        {
            if (MGame.Instance.Multiplayer && !player.GetComponent<PhotonView>().IsMine) continue;
            if (player.position.x == mx && player.position.y == my)
            {
                player.GetComponent<CPlayer>().DestroyPlayer();
            }
        }
    }
    [PunRPC]
    public void RemovePlayerAtPosIntersect(float mx, float my)
    {
        float[] dx = { -0.5f, 0, 0.5f, -0.5f, 0, 0.5f, -0.5f, 0, 0.5f };
        float[] dy = { -0.5f, -0.5f, -0.5f, 0, 0, 0, 0.5f, 0.5f, 0.5f };
        for(int i = 0; i < dx.Length; i++)
        {
            RemovePlayerAtPos(mx + dx[i], my + dy[i]);
        }
    }

    public static bool CanPlace(float mx, float my)
    {
        // conditions: no player there, position is covered with possible start fields
        return !IsPlayerThere(mx, my) && MField.IsPosCoveredWithFieldType(mx, my, StartFields.ToArray());
    }

    public static int AvailableID()
    {
        if (GetPlayers().Count == 0) return 0;

        int highestID = 0;
        foreach (Transform p in MGame.Instance.PlayerContainer.transform)
        {
            CPlayer controller = p.GetComponent<CPlayer>();
            if (controller.id > highestID) highestID = controller.id;
        }
        return highestID + 1;
    }

    #region Get player
    public static GameObject GetClientPlayer()
    {
        if (!MGame.Instance.Multiplayer) throw new System.Exception("Trying to acces player of client while singleplayer");

        List<GameObject> players = GetPlayers();
        foreach(GameObject player in players)
        {
            CPlayer controller = player.GetComponent<CPlayer>();
            if (controller.photonView.IsMine) return player;
        }
        return null;
    }


    public static List<GameObject> GetPlayers()
    {
        GameObject container = MGame.Instance.PlayerContainer;
        List<GameObject> players = new();
        for(int i = 0; i < container.transform.childCount; i++)
        {
            players.Add(container.transform.GetChild(i).gameObject);
        }
        return players;
    }
    public static GameObject GetPlayer(float mx, float my)
    {
        List<GameObject> players = GetPlayers();
        foreach(GameObject player in players)
        {
            if (MGame.Instance.Multiplayer && !player.GetComponent<PhotonView>().IsMine) continue;
            if (player.transform.position.x == mx && player.transform.position.y == my) return player;
        }
        return null;
    }
    public static GameObject GetPlayer()
    {
        if (MGame.Instance.Multiplayer) return GetClientPlayer();

        // getting the one player in singleplayer
        GameObject container = MGame.Instance.PlayerContainer;
        if (container.transform.childCount > 1) throw new System.Exception("There are multiple player objects within GameManager.PlayerContainer while trying to access the specific player in singleplayer");

        try { return container.transform.GetChild(0).gameObject; }
        catch (System.Exception) { return null; }
    }
    public static GameObject GetPlayer(int id) { return PlayerIDList()[id]; }
    public static bool IsPlayerThere(float mx, float my) { return GetPlayer(mx, my) != null; }
    public static bool IsPlayerThereIntersect(float mx, float my)
    {
        float[] dx = { -0.5f, 0, 0.5f, -0.5f, 0, 0.5f, -0.5f, 0, 0.5f };
        float[] dy = { -0.5f, -0.5f, -0.5f, 0, 0, 0, 0.5f, 0.5f, 0.5f };
        for (int i = 0; i < dx.Length; i++)
        {
            if (IsPlayerThere(mx + dx[i], my + dy[i])) return true;
        }
        return false;
    }
    #endregion

    public static Dictionary<int, GameObject> PlayerIDList()
    {
        Dictionary<int, GameObject> res = new();
        foreach (Transform p in MGame.Instance.PlayerContainer.transform)
        {
            CPlayer controller = p.GetComponent<CPlayer>();
            res.Add(controller.id, p.gameObject);
        }
        return res;
    }

    public static GameObject InstantiatePlayer(Vector2 pos, float speed, bool multiplayer)
    {
        GameObject newPlayer;
        if (multiplayer)
        {
            newPlayer = PhotonNetwork.Instantiate(MGame.Instance.Player.name, pos, Quaternion.identity);

            PhotonView view = newPlayer.GetComponent<PhotonView>();
            view.RPC("SetSpeed", RpcTarget.All, speed);
        }
        else
        {
            newPlayer = Instantiate(MGame.Instance.Player, pos, Quaternion.identity, MGame.Instance.PlayerContainer.transform);
            
            CPlayer controller_ = newPlayer.GetComponent<CPlayer>();
            controller_.SetSpeed(speed);
        }

        return newPlayer;
    }
    public static GameObject InstantiatePlayer(float mx, float my, float speed, bool multiplayer)
    {
        return InstantiatePlayer(new(mx, my), speed ,multiplayer);
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}
