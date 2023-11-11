using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    // list of fields which are safe for player
    public static readonly List<FieldType> SafeFields = new(new FieldType[] { });

    public static readonly List<FieldType> StartFields = new(
        new[]
        {
            FieldType.StartField,
            FieldType.GoalField,
        }
    );
    
    public event Action OnWin;
    
    public void InvokeOnWin() => OnWin?.Invoke();

    #region Set player

    public void SetPlayer(Vector2 position, float speed, bool placeStartField = false)
    {
        // TODO: improve
        if (!CanPlace(position))
        {
            if (placeStartField)
            {
                Vector2Int[] checkPoses =
                {
                    Vector2Int.FloorToInt(position),
                    new(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y)),
                    new(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y)),
                    Vector2Int.CeilToInt(position),
                };

                foreach (Vector2Int checkPosition in checkPoses) FieldManager.Instance.SetField(checkPosition, FieldType.StartField);
            }
            else return;
        }

        // clear area from coins and keys
        GameManager.RemoveObjectInContainer(position, ReferenceManager.Instance.CoinContainer);
        GameManager.RemoveObjectInContainer(position, ReferenceManager.Instance.KeyContainer);

        // clear all players (only from this client tho)
        if (MultiplayerManager.Instance.Multiplayer)
        {
            foreach (Transform player in ReferenceManager.Instance.PlayerContainer)
            {
                PlayerController p = player.GetComponent<PlayerController>();
                PhotonView view = player.GetComponent<PhotonView>();

                // check if player is from own client
                if (!view.IsMine) continue;

                Vector2 playerPos = p.transform.position;

                // remove player
                GameManager.Instance.photonView.RPC(
                    "RemovePlayerAtPosOnlyOtherClients", RpcTarget.Others, playerPos.x,
                    playerPos.y
                );

                RemovePlayerAtPosIgnoreOtherClients(playerPos);
            }
        }
        else RemoveAllPlayers();

        // place player
        PlayerController newPlayer = InstantiatePlayer(position, speed, MultiplayerManager.Instance.Multiplayer);

        // int newID = AvailableID();
        // newPlayer.GetComponent<PlayerController>().ID = newID;

        // set target of camera
        ReferenceManager.Instance.MainCameraJumper.AddTarget("Player", newPlayer.gameObject);
    }

    public void SetPlayer(Vector2 position, bool placeStartField = false) => SetPlayer(position, 3f, placeStartField);

    #endregion

    [PunRPC]
    public void RemoveAllPlayers()
    {
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer) player.GetComponent<PlayerController>().DestroySelf();
    }

    [PunRPC]
    public void RemovePlayerAtPos(Vector2 position)
    {
        // remove player only if at pos
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer)
        {
            if ((Vector2)player.position == position) player.GetComponent<PlayerController>().DestroySelf();
        }
    }

    [PunRPC]
    public void RemovePlayerAtPosOnlyOtherClients(Vector2 position)
    {
        // remove player only if at pos
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer)
        {
            if (MultiplayerManager.Instance.Multiplayer && player.GetComponent<PhotonView>().IsMine) continue;
            if ((Vector2)player.position == position) player.GetComponent<PlayerController>().DestroySelf();
        }
    }

    [PunRPC]
    public void RemovePlayerAtPosIgnoreOtherClients(Vector2 position)
    {
        // remove player only if at pos
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer)
        {
            if ((Vector2)player.position == position) player.GetComponent<PlayerController>().DestroySelf();
        }
    }

    public void RemovePlayerAtPosIntersect(Vector2 position)
    {
        Vector2[] deltas =
        {
            new(-0.5f, -0.5f), new(0, -0.5f), new(0.5f, -0.5f),
            new(-0.5f, 0), new(0, 0), new(0.5f, 0),
            new(-0.5f, 0.5f), new(0, 0.5f), new(0.5f, 0.5f),
        };

        foreach (Vector2 d in deltas) RemovePlayerAtPos(position + d);
    }

    public static bool CanPlace(Vector2 position, bool checkForPlayer = true) =>
        // conditions: no player there, position is covered with possible start fields
        !(checkForPlayer && IsPlayerThere(position)) &&
        FieldManager.IsPosCoveredWithFieldType(position, StartFields.ToArray());

    // public static int AvailableID()
    // {
    //     if (GetPlayers().Count == 0) return 0;
    //
    //     int highestID = 0;
    //     foreach (Transform p in ReferenceManager.Instance.PlayerContainer)
    //     {
    //         PlayerController controller = p.GetComponent<PlayerController>();
    //         if (controller.ID > highestID) highestID = controller.ID;
    //     }
    //
    //     return highestID + 1;
    // }

    #region Get player

    public static PlayerController GetClientPlayer()
    {
        if (!MultiplayerManager.Instance.Multiplayer) throw new Exception("Trying to access player of client while singleplayer");

        List<PlayerController> players = GetPlayers();
        foreach (PlayerController controller in players)
        {
            if (controller.PhotonView.IsMine) return controller;
        }

        return null;
    }

    public static List<PlayerController> GetPlayers()
    {
        Transform container = ReferenceManager.Instance.PlayerContainer;
        List<PlayerController> players = new();

        foreach (Transform player in container)
        {
            if (player.TryGetComponent(out PlayerController controller)) players.Add(controller);
            else Debug.LogWarning("Could not find PlayerController component in player");
        }
        
        return players;
    }

    public static PlayerController GetPlayer(Vector2 position)
    {
        List<PlayerController> players = GetPlayers();
        foreach (PlayerController player in players)
        {
            if (MultiplayerManager.Instance.Multiplayer && !player.PhotonView.IsMine) continue;
            if ((Vector2)player.transform.position == position) return player;
        }

        return null;
    }

    public static PlayerController GetPlayer()
    {
        if (MultiplayerManager.Instance.Multiplayer) return GetClientPlayer();

        // getting the one player in single player
        // Transform container = ReferenceManager.Instance.PlayerContainer;
        List<PlayerController> players = GetPlayers();
        if (players.Count > 1)
        {
            throw new Exception(
                "There are multiple player objects within GameManager.PlayerContainer while trying to access the specific player in singleplayer"
            );
        }

        return players.Count == 0 ? null : players[0];
    }

    // public static GameObject GetPlayer(int id) => PlayerIDList()[id];

    public static bool IsPlayerThere(Vector2 position) => GetPlayer(position) != null;

    public static bool IsPlayerThereIntersect(Vector2 position)
    {
        Vector2[] deltas =
        {
            new(-0.5f, -0.5f), new(0, -0.5f), new(0.5f, -0.5f),
            new(-0.5f, 0), new(0, 0), new(0.5f, 0),
            new(-0.5f, 0.5f), new(0, 0.5f), new(0.5f, 0.5f),
        };

        foreach (Vector2 d in deltas)
        {
            if (IsPlayerThere(position + d)) return true;
        }

        return false;
    }

    #endregion

    // public static Dictionary<int, GameObject> PlayerIDList()
    // {
    //     Dictionary<int, GameObject> res = new();
    //     foreach (Transform p in ReferenceManager.Instance.PlayerContainer)
    //     {
    //         PlayerController controller = p.GetComponent<PlayerController>();
    //         res.Add(controller.ID, p.gameObject);
    //     }
    //
    //     return res;
    // }

    public static PlayerController InstantiatePlayer(Vector2 position, float speed, bool multiplayer)
    {
        PlayerController newPlayer;
        if (multiplayer)
        {
            GameObject newPlayerObject = PhotonNetwork.Instantiate(
                PrefabManager.Instance.Player.name, position,
                Quaternion.identity
            );

            newPlayer = newPlayerObject.GetComponent<PlayerController>();

            newPlayer.PhotonView.RPC("SetSpeed", RpcTarget.All, speed);
        }
        else
        {
            newPlayer = Instantiate(
                PrefabManager.Instance.Player, position, Quaternion.identity,
                ReferenceManager.Instance.PlayerContainer
            );
            
            newPlayer.SetSpeed(speed);
        }

        return newPlayer;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    public void ResetStates()
    {
        List<PlayerController> players = GetPlayers();
        
        // reset players
        foreach (PlayerController player in players)
        {
            if (MultiplayerManager.Instance.Multiplayer && !player.PhotonView.IsMine) continue;
            player.DieNormal();
            player.CoinsCollected.Clear();
            player.KeysCollected.Clear();
            player.CurrentGameState = null;
        }
    }

    public void Setup()
    {
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            if (MultiplayerManager.Instance.Multiplayer && !controller.PhotonView.IsMine) continue;

            controller.CurrentFields.Clear();
            controller.CurrentGameState = null;
            controller.Deaths = 0;
        }
    }
}