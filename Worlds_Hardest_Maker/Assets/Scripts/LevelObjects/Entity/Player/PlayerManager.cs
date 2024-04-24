using System;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IManager<PlayerController>, IManagerPlaceRestrictable
{
    public static PlayerManager Instance { get; private set; }

    public event Action OnWin;

    public void InvokeOnWin() => OnWin?.Invoke();

    [ReadOnly] public PlayerController Player;

    public Transform DefaultContainer => ReferenceManager.Instance.PlayerContainer;

    public PlayerController SetInSheet(ManagerParameters args)
    {
        Vector2 position = args.Position;

        if (IsThereInSheet(position, args.Sheet)) return null;

        if (args.SurroundWithStartFields && !CanPlaceInSheet(position, args.Sheet)) SetSurroundingStartFieldsInSheet(position, args.Sheet);

        // clear area from coins and keys
        GameManager.RemoveObjectInContainer(position, ReferenceManager.Instance.CoinContainer);
        GameManager.RemoveObjectInContainer(position, ReferenceManager.Instance.KeyContainer);

        // if player already exists, just move it
        if (Player != null)
        {
            Player.transform.position = position;
            Player.StartPos = position;
        }
        else
        {
            // place player
            PlayerController newPlayer = ((IManager<PlayerController>)this).InstantiateInSheet(args);

            // set target of camera
            ReferenceManager.Instance.MainCameraJumper.SetTarget("Player", newPlayer.gameObject);

            Player = newPlayer;
        }

        PlaceManager.AttachToSheet(Player.gameObject, args.Sheet, false);
        Player.Sheet = args.Sheet;

        return Player;
    }

    // public PlayerController Set(ManagerParameters args)
    // {
    //     Vector2 position = args.Position;
    //     
    //     if (IsThere(position)) return null;
    //
    //     if (!CanPlace(position))
    //     {
    //         if (!args.SurroundWithStartFields) return null;
    //
    //         SetSurroundingStartFields(position);
    //     }
    //
    //     // clear area from coins and keys
    //     GameManager.RemoveObjectInContainer(position, ReferenceManager.Instance.CoinContainer);
    //     GameManager.RemoveObjectInContainer(position, ReferenceManager.Instance.KeyContainer);
    //     
    //     AnchorController sheet = PlaceManager.GetCurrentSheet();
    //     
    //     // if player already exists, just move it
    //     if (Player != null)
    //     {
    //         Player.transform.position = position;
    //         Player.StartPos = position;
    //     }
    //     else
    //     {
    //         // place player
    //         PlayerController newPlayer = ((IManager<PlayerController>)this).Instantiate(args);
    //
    //         // set target of camera
    //         ReferenceManager.Instance.MainCameraJumper.SetTarget("Player", newPlayer.gameObject);
    //
    //         Player = newPlayer;
    //     }
    //     
    //     PlaceManager.AttachToSheet(Player.gameObject, sheet, false);
    //     Player.Sheet = sheet;
    //
    //     return Player;
    // }
    public PlayerController Set(Vector2 position)
    {
        ManagerParameters args = new() { Position = position, SurroundWithStartFields = true, };
        return ((IManager<PlayerController>)this).Set(args);
    }

    public PlayerController GetInSheet(Vector2 position, AnchorController sheet) => throw new NotImplementedException();

    public PlayerController InstantiateInSheet(ManagerParameters args)
    {
        PlayerController newPlayer = Instantiate(
            PrefabManager.Instance.Player,
            args.Position, Quaternion.identity,
            DefaultContainer
        );

        PlaceManager.AttachToSheet(newPlayer.gameObject, args.Sheet, false);
        newPlayer.Sheet = args.Sheet;

        return newPlayer;
    }

    public bool IsThere(Vector2 position) => Instance.Player != null && (Vector2)Instance.Player.transform.position == position;
    public bool IsThereInSheet(Vector2 position, AnchorController sheet) => IsThere(position) && Instance.Player.Sheet == sheet;

    public bool CanPlace(Vector2 position) =>
        // conditions: no player there, position is covered with possible start fields
        !IsThere(position) &&
        FieldManager.Instance.IsPosCoveredWithFieldType(position, EditModeManager.Instance.AllPlayerStartFieldModes.ToArray());

    public bool CanPlaceInSheet(Vector2 position, AnchorController sheet) =>
        // conditions: no player there, position is covered with possible start fields
        !IsThereInSheet(position, sheet) &&
        FieldManager.Instance.IsPosCoveredWithFieldType(position, EditModeManager.Instance.AllPlayerStartFieldModes.ToArray());

    private static void SetSurroundingStartFields(Vector2 position)
    {
        Vector2Int[] checkPoses =
        {
            Vector2Int.FloorToInt(position),
            new(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y)),
            new(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y)),
            Vector2Int.CeilToInt(position),
        };

        foreach (Vector2Int checkPosition in checkPoses)
        {
            ManagerParameters args = new()
            {
                Position = checkPosition,
                FieldMode = EditModeManager.Start,
            };

            ((IManager<FieldController>)FieldManager.Instance).Set(args);
        }
    }

    private static void SetSurroundingStartFieldsInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        Vector2Int[] checkPoses =
        {
            Vector2Int.FloorToInt(position),
            new(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y)),
            new(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y)),
            Vector2Int.CeilToInt(position),
        };

        foreach (Vector2Int checkPosition in checkPoses)
        {
            ManagerParameters args = new()
            {
                Position = checkPosition,
                FieldMode = EditModeManager.Start,
            };

            ((IManager<FieldController>)FieldManager.Instance).Set(args);
        }
    }

    public void RemoveAtPos(Vector2 position)
    {
        // remove player only if at pos
        foreach (Transform player in DefaultContainer)
        {
            if ((Vector2)player.position == position) player.GetComponent<PlayerController>().DestroySelf();
        }
    }

    public void RemoveAtPosInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        // remove player only if at pos
        foreach (Transform p in DefaultContainer)
        {
            if ((Vector2)p.position != position) continue;

            PlayerController player = p.GetComponent<PlayerController>();

            if (IManager.IsInSheet(player, sheet)) player.DestroySelf();
        }
    }

    public void RemoveAtPosIntersect(Vector2 position)
    {
        Vector2[] deltas =
        {
            new(-0.5f, -0.5f), new(0, -0.5f), new(0.5f, -0.5f),
            new(-0.5f, 0), new(0, 0), new(0.5f, 0),
            new(-0.5f, 0.5f), new(0, 0.5f), new(0.5f, 0.5f),
        };

        foreach (Vector2 d in deltas) RemoveAtPos(position + d);
    }

    public void RemovePlayerAtPosIntersectInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        Vector2[] deltas =
        {
            new(-0.5f, -0.5f), new(0, -0.5f), new(0.5f, -0.5f),
            new(-0.5f, 0), new(0, 0), new(0.5f, 0),
            new(-0.5f, 0.5f), new(0, 0.5f), new(0.5f, 0.5f),
        };

        foreach (Vector2 d in deltas) RemoveAtPosInSheet(position + d, sheet);
    }

    public bool IsThereIntersect(Vector2 position)
    {
        Vector2[] deltas =
        {
            new(-0.5f, -0.5f), new(0, -0.5f), new(0.5f, -0.5f),
            new(-0.5f, 0), new(0, 0), new(0.5f, 0),
            new(-0.5f, 0.5f), new(0, 0.5f), new(0.5f, 0.5f),
        };

        foreach (Vector2 d in deltas)
        {
            if (IsThere(position + d)) return true;
        }

        return false;
    }

    public static Vector2Int GetCurrentRoom() => Instance.Player != null ? Instance.Player.GetCurrentRoom() : Vector2Int.zero;
    public static Vector2Int GetStartRoom() => Instance.Player != null ? Instance.Player.GetStartRoom() : Vector2Int.zero;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    public bool CorrespondsToEditMode(EditMode compare) => compare == EditModeManager.Player;
}