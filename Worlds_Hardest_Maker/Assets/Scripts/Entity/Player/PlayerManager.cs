using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

/// <summary>
/// manages player duh
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    // list of fields which are safe for player
    public static readonly List<FieldType> SafeFields = new(new FieldType[]{
        
    });
    public static readonly List<FieldType> StartFields = new(new[]
    {
        FieldType.START_FIELD,
        FieldType.GOAL_FIELD
    });

    public event Action OnWin;

    public void InvokeOnWin()
    {
        OnWin?.Invoke();
    }

    #region Set player
    public void SetPlayer(float mx, float my, float speed, bool placeStartField = false)
    {
        // TODO: improve
        if (!CanPlace(mx, my))
        {
            if (placeStartField)
            {
                (int, int)[] poses =
                {
                    (Mathf.FloorToInt(mx), Mathf.FloorToInt(my)),
                    (Mathf.CeilToInt(mx), Mathf.FloorToInt(my)),
                    (Mathf.FloorToInt(mx), Mathf.CeilToInt(my)),
                    (Mathf.CeilToInt(mx), Mathf.CeilToInt(my))
                };

                foreach((int x, int y) in poses)
                {
                    FieldManager.Instance.SetField(x, y, FieldType.START_FIELD);
                }
            }
            else return;
        }

        // clear area from coins and keys
        GameManager.RemoveObjectInContainer(mx, my, ReferenceManager.Instance.CoinContainer);
        GameManager.RemoveObjectInContainer(mx, my, ReferenceManager.Instance.KeyContainer);

        // clear all players (only from this client tho)
        if (GameManager.Instance.Multiplayer)
        {
            foreach (Transform player in ReferenceManager.Instance.PlayerContainer)
            {
                PlayerController p = player.GetComponent<PlayerController>();
                PhotonView view = player.GetComponent<PhotonView>();

                // check if player is from own client
                if (!view.IsMine) continue;

                Vector2 playerPos = p.transform.position;

                // remove player
                GameManager.Instance.photonView.RPC("RemovePlayerAtPosOnlyOtherClients", RpcTarget.Others, playerPos.x, playerPos.y);
                RemovePlayerAtPosIgnoreOtherClients(playerPos.x, playerPos.y);
            }
        }
        else
        {
            RemoveAllPlayers();
        }

        // place player
        GameObject newPlayer = InstantiatePlayer(mx, my, speed, GameManager.Instance.Multiplayer);

        int newID = AvailableID();
        newPlayer.GetComponent<PlayerController>().id = newID;

        // set target of camera
        if (Camera.main != null) Camera.main.GetComponent<JumpToEntity>().target = newPlayer;
    }

    public void SetPlayer(Vector2 pos, float speed, bool placeStartField = false)
    {
        SetPlayer(pos.x, pos.y, speed, placeStartField);
    }
    [PunRPC]
    public void SetPlayer(float mx, float my, bool placeStartField = false)
    {
        SetPlayer(mx, my, 3f, placeStartField);
    }
    #endregion

    [PunRPC]
    public void RemoveAllPlayers()
    {
        foreach(Transform player in ReferenceManager.Instance.PlayerContainer)
        {
            player.GetComponent<PlayerController>().DestroyPlayer();
        }
    }
    [PunRPC]
    public void RemovePlayerAtPos(float mx, float my)
    {
        // remove player only if at pos
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer)
        {
            if(player.position.x == mx && player.position.y == my)
            {
                player.GetComponent<PlayerController>().DestroyPlayer();
            }
        }
    }

    public void RemovePlayerAtPos(Vector2 pos)
    {
        RemovePlayerAtPos(pos.x, pos.y);
    }
    [PunRPC]
    public void RemovePlayerAtPosOnlyOtherClients(float mx, float my)
    {
        // remove player only if at pos
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer)
        {
            if (GameManager.Instance.Multiplayer && player.GetComponent<PhotonView>().IsMine) continue;
            if (player.position.x == mx && player.position.y == my)
            {
                player.GetComponent<PlayerController>().DestroyPlayer();
            }
        }
    }
    [PunRPC]
    public void RemovePlayerAtPosIgnoreOtherClients(float mx, float my)
    {
        // remove player only if at pos
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer)
        {
            if (GameManager.Instance.Multiplayer && !player.GetComponent<PhotonView>().IsMine) continue;
            if (player.position.x == mx && player.position.y == my)
            {
                player.GetComponent<PlayerController>().DestroyPlayer();
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

    public static bool CanPlace(float mx, float my, bool checkForPlayer = true)
    {
        // conditions: no player there, position is covered with possible start fields
        return !(checkForPlayer && IsPlayerThere(mx, my)) && FieldManager.IsPosCoveredWithFieldType(mx, my, StartFields.ToArray());
    }

    public static bool CanPlace(Vector2 pos, bool checkForPlayer = true)
    {
        return CanPlace(pos.x, pos.y, checkForPlayer);
    }

    public static int AvailableID()
    {
        if (GetPlayers().Count == 0) return 0;

        int highestID = 0;
        foreach (Transform p in ReferenceManager.Instance.PlayerContainer)
        {
            PlayerController controller = p.GetComponent<PlayerController>();
            if (controller.id > highestID) highestID = controller.id;
        }
        return highestID + 1;
    }

    #region Get player
    public static GameObject GetClientPlayer()
    {
        if (!GameManager.Instance.Multiplayer) throw new System.Exception("Trying to acces player of client while singleplayer");

        List<GameObject> players = GetPlayers();
        foreach(GameObject player in players)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller.photonView.IsMine) return player;
        }
        return null;
    }
    public static List<GameObject> GetPlayers()
    {
        Transform container = ReferenceManager.Instance.PlayerContainer;
        List<GameObject> players = new();
        for(int i = 0; i < container.childCount; i++)
        {
            players.Add(container.GetChild(i).gameObject);
        }
        return players;
    }
    public static GameObject GetPlayer(float mx, float my)
    {
        List<GameObject> players = GetPlayers();
        foreach(GameObject player in players)
        {
            if (GameManager.Instance.Multiplayer && !player.GetComponent<PhotonView>().IsMine) continue;
            if (player.transform.position.x == mx && player.transform.position.y == my) return player;
        }
        return null;
    }
    public static GameObject GetPlayer()
    {
        if (GameManager.Instance.Multiplayer) return GetClientPlayer();

        // getting the one player in single player
        Transform container = ReferenceManager.Instance.PlayerContainer;
        if (container.transform.childCount > 1) throw new Exception("There are multiple player objects within GameManager.PlayerContainer while trying to access the specific player in singleplayer");

        try { return container.GetChild(0).gameObject; }
        catch (Exception) { return null; }
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
        foreach (Transform p in ReferenceManager.Instance.PlayerContainer)
        {
            PlayerController controller = p.GetComponent<PlayerController>();
            res.Add(controller.id, p.gameObject);
        }
        return res;
    }

    public static GameObject InstantiatePlayer(Vector2 pos, float speed, bool multiplayer)
    {
        GameObject newPlayer;
        if (multiplayer)
        {
            newPlayer = PhotonNetwork.Instantiate(PrefabManager.Instance.Player.name, pos, Quaternion.identity);

            PhotonView view = newPlayer.GetComponent<PhotonView>();
            view.RPC("SetSpeed", RpcTarget.All, speed);
        }
        else
        {
            newPlayer = Instantiate(PrefabManager.Instance.Player, pos, Quaternion.identity, ReferenceManager.Instance.PlayerContainer);
            
            PlayerController controller_ = newPlayer.GetComponent<PlayerController>();
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
