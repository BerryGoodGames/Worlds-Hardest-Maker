using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     Extension for EditMode methods and readability
/// </summary>
public static class EditModeExtension
{
    private static readonly Dictionary<EditMode, GameObject> editModeToPrefab = new()
    {
        { EditMode.Wall, PrefabManager.Instance.Wall },
        { EditMode.Start, PrefabManager.Instance.Start },
        { EditMode.Goal, PrefabManager.Instance.Goal },
        { EditMode.Checkpoint, PrefabManager.Instance.Checkpoint },
        { EditMode.OneWay, PrefabManager.Instance.OneWay },
        { EditMode.Conveyor, PrefabManager.Instance.Conveyor },
        { EditMode.Water, PrefabManager.Instance.Water },
        { EditMode.Ice, PrefabManager.Instance.Ice },
        { EditMode.Void, PrefabManager.Instance.Void },
        { EditMode.GrayKeyDoor, PrefabManager.Instance.GrayKeyDoor },
        { EditMode.RedKeyDoor, PrefabManager.Instance.RedKeyDoor },
        { EditMode.GreenKeyDoor, PrefabManager.Instance.GreenKeyDoor },
        { EditMode.BlueKeyDoor, PrefabManager.Instance.BlueKeyDoor },
        { EditMode.YellowKeyDoor, PrefabManager.Instance.YellowKeyDoor },
        { EditMode.Player, PrefabManager.Instance.Player.gameObject },
        { EditMode.Anchor, PrefabManager.Instance.Anchor },
        { EditMode.AnchorBall, PrefabManager.Instance.AnchorBall },
        { EditMode.Coin, PrefabManager.Instance.Coin.gameObject },
        { EditMode.GrayKey, PrefabManager.Instance.GrayKey.gameObject },
        { EditMode.RedKey, PrefabManager.Instance.RedKey.gameObject },
        { EditMode.GreenKey, PrefabManager.Instance.GreenKey.gameObject },
        { EditMode.BlueKey, PrefabManager.Instance.BlueKey.gameObject },
        { EditMode.YellowKey, PrefabManager.Instance.YellowKey.gameObject },
    };

    private static readonly string[] uiStrings = 
    {
        "Delete",
        "Wall Field",
        "Start Field",
        "Goal Field",
        "Checkpoint Field",
        "One Way Gate",
        "Conveyor",
        "Water",
        "Ice",
        "Void",
        "Key Door Field - Gray",
        "Key Door Field - Red",
        "Key Door Field - Green",
        "Key Door Field - Blue",
        "Key Door Field - Yellow",
        "Player",
        "Anchor",
        "Ball",
        "Coin",
        "Key - Gray",
        "Key - Red",
        "Key - Green",
        "Key - Blue",
        "Key - Yellow",
    };

    private static readonly EditMode[] fieldTypes =
    {
        EditMode.Wall,
        EditMode.Start,
        EditMode.Goal,
        EditMode.Checkpoint,
        EditMode.OneWay,
        EditMode.Conveyor,
        EditMode.Water,
        EditMode.Ice,
        EditMode.Void,
        EditMode.GrayKeyDoor,
        EditMode.RedKeyDoor,
        EditMode.GreenKeyDoor,
        EditMode.BlueKeyDoor,
        EditMode.YellowKeyDoor,
    };

    private static readonly EditMode[] notDraggable =
    {
        EditMode.Anchor,
    };
    
    public static WorldPositionType GetWorldPositionType(this EditMode mode)
    {
        if (mode.IsFieldType() || mode == EditMode.Delete) return WorldPositionType.Matrix;
        return WorldPositionType.Grid;
    }

    public static GameObject GetPrefab(this EditMode mode) => editModeToPrefab[mode];

    public static string GetUIString(this EditMode mode) => uiStrings[(int)mode];

    public static bool IsFieldType(this EditMode mode) => fieldTypes.Contains(mode);

    public static bool IsKey(this EditMode mode) => KeyManager.KeyModes.Contains(mode);

    public static bool IsKeyDoor(this EditMode mode) => KeyManager.KeyDoorModes.Contains(mode);

    public static bool IsDraggable(this EditMode mode) => !notDraggable.Contains(mode);

    public static bool IsAnchorRelated(this EditMode mode) => mode is EditMode.Anchor or EditMode.AnchorBall;
}