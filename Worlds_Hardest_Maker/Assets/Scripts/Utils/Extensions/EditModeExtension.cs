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
        { EditMode.WallField, PrefabManager.Instance.WallField },
        { EditMode.StartField, PrefabManager.Instance.StartField },
        { EditMode.GoalField, PrefabManager.Instance.GoalField },
        { EditMode.CheckpointField, PrefabManager.Instance.CheckpointField },
        { EditMode.OneWayField, PrefabManager.Instance.OneWayField },
        { EditMode.Conveyor, PrefabManager.Instance.Conveyor },
        { EditMode.Water, PrefabManager.Instance.Water },
        { EditMode.Ice, PrefabManager.Instance.Ice },
        { EditMode.Void, PrefabManager.Instance.Void },
        { EditMode.GrayKeyDoorField, PrefabManager.Instance.GrayKeyDoorField },
        { EditMode.RedKeyDoorField, PrefabManager.Instance.RedKeyDoorField },
        { EditMode.GreenKeyDoorField, PrefabManager.Instance.GreenKeyDoorField },
        { EditMode.BlueKeyDoorField, PrefabManager.Instance.BlueKeyDoorField },
        { EditMode.YellowKeyDoorField, PrefabManager.Instance.YellowKeyDoorField },
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
        EditMode.WallField,
        EditMode.StartField,
        EditMode.GoalField,
        EditMode.CheckpointField,
        EditMode.OneWayField,
        EditMode.Conveyor,
        EditMode.Water,
        EditMode.Ice,
        EditMode.Void,
        EditMode.GrayKeyDoorField,
        EditMode.RedKeyDoorField,
        EditMode.GreenKeyDoorField,
        EditMode.BlueKeyDoorField,
        EditMode.YellowKeyDoorField,
    };

    private static readonly EditMode[] notDraggable =
    {
        EditMode.Anchor,
    };
    
    public static WorldPositionType GetWorldPositionType(this EditMode mode)
    {
        if (mode.IsFieldType() || mode == EditMode.DeleteField) return WorldPositionType.Matrix;
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