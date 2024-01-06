using System;
using MyBox;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public event Action OnWin;

    public void InvokeOnWin() => OnWin?.Invoke();

    [ReadOnly] public PlayerController Player;

    #region Set player

    public PlayerController SetPlayer(Vector2 position, float speed, bool surroundWithStartFields = false)
    {
        if (IsPlayerThere(position)) return null;

        // TODO: improve
        if (!CanPlace(position))
        {
            if (!surroundWithStartFields) return null;

            SetSurroundingStartFields(position);
        }

        // clear area from coins and keys
        GameManager.RemoveObjectInContainer(position, ReferenceManager.Instance.CoinContainer);
        GameManager.RemoveObjectInContainer(position, ReferenceManager.Instance.KeyContainer);

        // if player already exists, just move it
        if (Player != null)
        {
            Player.transform.position = position;
            return Player;
        }

        // place player
        PlayerController newPlayer = InstantiatePlayer(position, speed);

        // set target of camera
        ReferenceManager.Instance.MainCameraJumper.SetTarget("Player", newPlayer.gameObject);

        Player = newPlayer;

        return newPlayer;
    }

    private static void SetSurroundingStartFields(Vector2 position)
    {
        Vector2Int[] checkPoses =
        {
            Vector2Int.FloorToInt(position),
            new(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y)),
            new(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y)),
            Vector2Int.CeilToInt(position),
        };

        foreach (Vector2Int checkPosition in checkPoses) FieldManager.Instance.SetField(checkPosition, EditModeManager.Start);
    }

    public PlayerController SetPlayer(Vector2 position, bool placeStartField = false) => SetPlayer(position, 3f, placeStartField);

    #endregion
    
    public void RemovePlayerAtPos(Vector2 position)
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
        FieldManager.IsPosCoveredWithFieldType(position, EditModeManager.Instance.AllPlayerStartFieldModes.ToArray());

    public static bool IsPlayerThere(Vector2 position) => Instance.Player != null && (Vector2)Instance.Player.transform.position == position;

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

    public static PlayerController InstantiatePlayer(Vector2 position, float speed)
    {
        PlayerController newPlayer = Instantiate(
            PrefabManager.Instance.Player, position, Quaternion.identity,
            ReferenceManager.Instance.PlayerContainer
        );

        newPlayer.SetSpeed(speed);

        return newPlayer;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}