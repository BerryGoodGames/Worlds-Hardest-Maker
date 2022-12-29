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
            { EditMode.WALL_FIELD, PrefabManager.Instance.wallField },
            { EditMode.START_FIELD, PrefabManager.Instance.startField },
            { EditMode.GOAL_FIELD, PrefabManager.Instance.goalField },
            { EditMode.CHECKPOINT_FIELD, PrefabManager.Instance.checkpointField },
            { EditMode.ONE_WAY_FIELD, PrefabManager.Instance.oneWayField },
            { EditMode.CONVEYOR, PrefabManager.Instance.conveyor },
            { EditMode.WATER, PrefabManager.Instance.water },
            { EditMode.ICE, PrefabManager.Instance.ice },
            { EditMode.VOID, PrefabManager.Instance.@void },
            { EditMode.GRAY_KEY_DOOR_FIELD, PrefabManager.Instance.grayKeyDoorField },
            { EditMode.RED_KEY_DOOR_FIELD, PrefabManager.Instance.redKeyDoorField },
            { EditMode.GREEN_KEY_DOOR_FIELD, PrefabManager.Instance.greenKeyDoorField },
            { EditMode.BLUE_KEY_DOOR_FIELD, PrefabManager.Instance.blueKeyDoorField },
            { EditMode.YELLOW_KEY_DOOR_FIELD, PrefabManager.Instance.yellowKeyDoorField },
            { EditMode.PLAYER, PrefabManager.Instance.player },
            { EditMode.ANCHOR, PrefabManager.Instance.anchor },
            { EditMode.BALL, PrefabManager.Instance.ball },
            { EditMode.BALL_DEFAULT, PrefabManager.Instance.ballDefault },
            { EditMode.BALL_CIRCLE, PrefabManager.Instance.ballCircle },
            { EditMode.COIN, PrefabManager.Instance.coin },
            { EditMode.GRAY_KEY, PrefabManager.Instance.grayKey },
            { EditMode.RED_KEY, PrefabManager.Instance.redKey },
            { EditMode.GREEN_KEY, PrefabManager.Instance.greenKey },
            { EditMode.BLUE_KEY, PrefabManager.Instance.blueKey },
            { EditMode.YELLOW_KEY, PrefabManager.Instance.yellowKey }
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