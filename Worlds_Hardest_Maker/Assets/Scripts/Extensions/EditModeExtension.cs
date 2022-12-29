using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     extension for EditMode methods and readability
/// </summary>
public static class EditModeExtension
{
    public static FollowMouse.WorldPosition GetWorldPosition(this EditMode mode)
    {
        if (mode.IsFieldType() || mode == EditMode.DELETE_FIELD) return FollowMouse.WorldPosition.MATRIX;
        return FollowMouse.WorldPosition.GRID;
    }

    public static GameObject GetPrefab(this EditMode mode)
    {
        Dictionary<EditMode, GameObject> prefabs = new()
        {
            { EditMode.WALL_FIELD, PrefabManager.Instance.WallField },
            { EditMode.START_FIELD, PrefabManager.Instance.StartField },
            { EditMode.GOAL_FIELD, PrefabManager.Instance.GoalField },
            { EditMode.CHECKPOINT_FIELD, PrefabManager.Instance.CheckpointField },
            { EditMode.ONE_WAY_FIELD, PrefabManager.Instance.OneWayField },
            { EditMode.CONVEYOR, PrefabManager.Instance.Conveyor },
            { EditMode.WATER, PrefabManager.Instance.Water },
            { EditMode.ICE, PrefabManager.Instance.Ice },
            { EditMode.VOID, PrefabManager.Instance.Void },
            { EditMode.GRAY_KEY_DOOR_FIELD, PrefabManager.Instance.GrayKeyDoorField },
            { EditMode.RED_KEY_DOOR_FIELD, PrefabManager.Instance.RedKeyDoorField },
            { EditMode.GREEN_KEY_DOOR_FIELD, PrefabManager.Instance.GreenKeyDoorField },
            { EditMode.BLUE_KEY_DOOR_FIELD, PrefabManager.Instance.BlueKeyDoorField },
            { EditMode.YELLOW_KEY_DOOR_FIELD, PrefabManager.Instance.YellowKeyDoorField },
            { EditMode.PLAYER, PrefabManager.Instance.Player },
            { EditMode.ANCHOR, PrefabManager.Instance.Anchor },
            { EditMode.BALL, PrefabManager.Instance.Ball },
            { EditMode.BALL_DEFAULT, PrefabManager.Instance.BallDefault },
            { EditMode.BALL_CIRCLE, PrefabManager.Instance.BallCircle },
            { EditMode.COIN, PrefabManager.Instance.Coin },
            { EditMode.GRAY_KEY, PrefabManager.Instance.GrayKey },
            { EditMode.RED_KEY, PrefabManager.Instance.RedKey },
            { EditMode.GREEN_KEY, PrefabManager.Instance.GreenKey },
            { EditMode.BLUE_KEY, PrefabManager.Instance.BlueKey },
            { EditMode.YELLOW_KEY, PrefabManager.Instance.YellowKey }
        };
        return prefabs[mode];
    }

    public static string GetUIString(this EditMode mode)
    {
        return new[]
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
    }

    public static bool IsFieldType(this EditMode mode)
    {
        // list of all field types
        List<EditMode> fieldModes = new()
        {
            EditMode.WALL_FIELD,
            EditMode.START_FIELD,
            EditMode.GOAL_FIELD,
            EditMode.CHECKPOINT_FIELD,
            EditMode.ONE_WAY_FIELD,
            EditMode.CONVEYOR,
            EditMode.WATER,
            EditMode.ICE,
            EditMode.VOID,
            EditMode.GRAY_KEY_DOOR_FIELD,
            EditMode.RED_KEY_DOOR_FIELD,
            EditMode.GREEN_KEY_DOOR_FIELD,
            EditMode.BLUE_KEY_DOOR_FIELD,
            EditMode.YELLOW_KEY_DOOR_FIELD
        };

        return fieldModes.Contains(mode);
    }
}