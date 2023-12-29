using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     Extension for EditMode methods and readability
/// </summary>
public static class EditModeExtension
{
    // private static readonly Dictionary<EditMode, GameObject> editModeToPrefab = new()
    // {
    //     { EditModeManager.Wall, PrefabManager.Instance.Wall },
    //     { EditModeManager.Start, PrefabManager.Instance.Start },
    //     { EditModeManager.Goal, PrefabManager.Instance.Goal },
    //     { EditModeManager.Checkpoint, PrefabManager.Instance.Checkpoint },
    //     { EditModeManager.OneWay, PrefabManager.Instance.OneWay },
    //     { EditModeManager.Conveyor, PrefabManager.Instance.Conveyor },
    //     { EditModeManager.Water, PrefabManager.Instance.Water },
    //     { EditModeManager.Ice, PrefabManager.Instance.Ice },
    //     { EditModeManager.Void, PrefabManager.Instance.Void },
    //     { EditModeManager.GrayKeyDoor, PrefabManager.Instance.GrayKeyDoor },
    //     { EditModeManager.RedKeyDoor, PrefabManager.Instance.RedKeyDoor },
    //     { EditModeManager.GreenKeyDoor, PrefabManager.Instance.GreenKeyDoor },
    //     { EditModeManager.BlueKeyDoor, PrefabManager.Instance.BlueKeyDoor },
    //     { EditModeManager.YellowKeyDoor, PrefabManager.Instance.YellowKeyDoor },
    //     { EditModeManager.Player, PrefabManager.Instance.Player.gameObject },
    //     { EditModeManager.Anchor, PrefabManager.Instance.Anchor },
    //     { EditModeManager.AnchorBall, PrefabManager.Instance.AnchorBall },
    //     { EditModeManager.Coin, PrefabManager.Instance.Coin.gameObject },
    //     { EditModeManager.GrayKey, PrefabManager.Instance.GrayKey.gameObject },
    //     { EditModeManager.RedKey, PrefabManager.Instance.RedKey.gameObject },
    //     { EditModeManager.GreenKey, PrefabManager.Instance.GreenKey.gameObject },
    //     { EditModeManager.BlueKey, PrefabManager.Instance.BlueKey.gameObject },
    //     { EditModeManager.YellowKey, PrefabManager.Instance.YellowKey.gameObject },
    // };

    // private static readonly string[] uiStrings = 
    // {
    //     "Delete",
    //     "Wall Field",
    //     "Start Field",
    //     "Goal Field",
    //     "Checkpoint Field",
    //     "One Way Gate",
    //     "Conveyor",
    //     "Water",
    //     "Ice",
    //     "Void",
    //     "Key Door Field - Gray",
    //     "Key Door Field - Red",
    //     "Key Door Field - Green",
    //     "Key Door Field - Blue",
    //     "Key Door Field - Yellow",
    //     "Player",
    //     "Anchor",
    //     "Ball",
    //     "Coin",
    //     "Key - Gray",
    //     "Key - Red",
    //     "Key - Green",
    //     "Key - Blue",
    //     "Key - Yellow",
    // };

    // private static readonly EditMode[] fieldTypes =
    // {
    //     EditModeManager.Wall,
    //     EditModeManager.Start,
    //     EditModeManager.Goal,
    //     EditModeManager.Checkpoint,
    //     EditModeManager.OneWay,
    //     EditModeManager.Conveyor,
    //     EditModeManager.Water,
    //     EditModeManager.Ice,
    //     EditModeManager.Void,
    //     EditModeManager.GrayKeyDoor,
    //     EditModeManager.RedKeyDoor,
    //     EditModeManager.GreenKeyDoor,
    //     EditModeManager.BlueKeyDoor,
    //     EditModeManager.YellowKeyDoor,
    // };

    // private static readonly EditMode[] notDraggable =
    // {
    //     EditModeManager.Anchor,
    // };
    
    public static WorldPositionType GetWorldPositionType(this EditMode mode)
    {
        if (mode.Attributes.IsField || mode == EditModeManager.Delete) return WorldPositionType.Matrix;
        return WorldPositionType.Grid;
    }

    // public static GameObject GetPrefab(this EditMode mode) => editModeToPrefab[mode];
    //
    // public static string GetUIString(this EditMode mode) => uiStrings[(int)mode];
    //
    // public static bool IsFieldType(this EditMode mode) => fieldTypes.Contains(mode);
    //
    // public static bool IsKey(this EditMode mode) => KeyManager.KeyModes.Contains(mode);
    //
    // public static bool IsKeyDoor(this EditMode mode) => KeyManager.KeyDoorModes.Contains(mode);
    //
    // public static bool IsDraggable(this EditMode mode) => !notDraggable.Contains(mode);

    // public static bool IsAnchorRelated(this EditMode mode) => mode == EditModeManager.Anchor || mode == EditModeManager.AnchorBall;
}