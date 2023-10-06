using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Extension for EditMode methods and readability
/// </summary>
public static class EditModeExtension
{
    public static WorldPositionType GetWorldPositionType(this EditMode mode)
    {
        if (mode.IsFieldType() || mode == EditMode.DeleteField) return WorldPositionType.Matrix;
        return WorldPositionType.Grid;
    }

    public static GameObject GetPrefab(this EditMode mode)
    {
        Dictionary<EditMode, GameObject> prefabs = new()
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
            { EditMode.Player, PrefabManager.Instance.Player },
            { EditMode.Anchor, PrefabManager.Instance.Anchor },
            { EditMode.AnchorBall, PrefabManager.Instance.AnchorBall },
            { EditMode.Coin, PrefabManager.Instance.Coin.gameObject },
            { EditMode.GrayKey, PrefabManager.Instance.GrayKey.gameObject },
            { EditMode.RedKey, PrefabManager.Instance.RedKey.gameObject },
            { EditMode.GreenKey, PrefabManager.Instance.GreenKey.gameObject },
            { EditMode.BlueKey, PrefabManager.Instance.BlueKey.gameObject },
            { EditMode.YellowKey, PrefabManager.Instance.YellowKey.gameObject }
        };
        return prefabs[mode];
    }

    public static string GetUIString(this EditMode mode) =>
        new[]
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
            "Key - Yellow"
        }[(int)mode];

    public static bool IsFieldType(this EditMode mode) =>
        // list of all field types
        new List<EditMode>
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
            EditMode.YellowKeyDoorField
        }.Contains(mode);

    public static bool IsKey(this EditMode mode) => KeyManager.KeyModes.Contains(mode);

    public static bool IsKeyDoor(this EditMode mode) => KeyManager.KeyDoorModes.Contains(mode);

    public static bool IsAnchorRelated(this EditMode mode) => mode is EditMode.Anchor or EditMode.AnchorBall;
}