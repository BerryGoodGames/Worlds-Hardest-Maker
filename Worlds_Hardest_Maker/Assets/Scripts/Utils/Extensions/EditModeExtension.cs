﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Extension for EditMode methods and readability
/// </summary>
public static class EditModeExtension
{
    public static FollowMouse.WorldPositionType GetWorldPosition(this EditMode mode)
    {
        if (mode.IsFieldType() || mode == EditMode.DeleteField) return FollowMouse.WorldPositionType.Matrix;
        return FollowMouse.WorldPositionType.Grid;
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
            { EditMode.BallDefault, PrefabManager.Instance.BallDefault },
            { EditMode.BallCircle, PrefabManager.Instance.BallCircle },
            { EditMode.Coin, PrefabManager.Instance.Coin },
            { EditMode.GrayKey, PrefabManager.Instance.GrayKey },
            { EditMode.RedKey, PrefabManager.Instance.RedKey },
            { EditMode.GreenKey, PrefabManager.Instance.GreenKey },
            { EditMode.BlueKey, PrefabManager.Instance.BlueKey },
            { EditMode.YellowKey, PrefabManager.Instance.YellowKey }
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
            "Ball - Line",
            "Ball - Circle",
            "Coin",
            "Key - Gray",
            "Key - Red",
            "Key - Green",
            "Key - Blue",
            "Key - Yellow"
        }[(int)mode];

    public static bool IsFieldType(this EditMode mode)
    {
        // list of all field types
        List<EditMode> fieldModes = new()
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
        };

        return fieldModes.Contains(mode);
    }

    public static bool IsAnchorRelated(this EditMode mode) => mode is EditMode.Anchor or EditMode.AnchorBall;
}