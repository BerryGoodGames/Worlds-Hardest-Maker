using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// extension for MGame.EditMode methods and readability
/// </summary>
public static class EditModeExtension
{
    public static FollowMouse.WorldPosition GetWorldPosition(this MGame.EditMode mode)
    {
        if (mode.IsFieldType() || mode == MGame.EditMode.DELETE_FIELD) return FollowMouse.WorldPosition.MATRIX;
        return FollowMouse.WorldPosition.GRID;
    }

    public static GameObject GetPrefab(this MGame.EditMode mode)
    {
        Dictionary<MGame.EditMode, GameObject> prefabs = new()
        {
            { MGame.EditMode.WALL_FIELD, MGame.Instance.WallField },
            { MGame.EditMode.START_FIELD, MGame.Instance.StartField },
            { MGame.EditMode.GOAL_FIELD, MGame.Instance.GoalField },
            { MGame.EditMode.CHECKPOINT_FIELD, MGame.Instance.CheckpointField },
            { MGame.EditMode.ONE_WAY_FIELD, MGame.Instance.OneWayField },
            { MGame.EditMode.CONVEYOR, MGame.Instance.Conveyor },
            { MGame.EditMode.WATER, MGame.Instance.Water },
            { MGame.EditMode.ICE, MGame.Instance.Ice },
            { MGame.EditMode.VOID, MGame.Instance.Void },
            { MGame.EditMode.GRAY_KEY_DOOR_FIELD, MGame.Instance.GrayKeyDoorField },
            { MGame.EditMode.RED_KEY_DOOR_FIELD, MGame.Instance.RedKeyDoorField },
            { MGame.EditMode.GREEN_KEY_DOOR_FIELD, MGame.Instance.GreenKeyDoorField },
            { MGame.EditMode.BLUE_KEY_DOOR_FIELD, MGame.Instance.BlueKeyDoorField },
            { MGame.EditMode.YELLOW_KEY_DOOR_FIELD, MGame.Instance.YellowKeyDoorField },
            { MGame.EditMode.PLAYER, MGame.Instance.Player },
            { MGame.EditMode.ANCHOR, MGame.Instance.Anchor },
            { MGame.EditMode.BALL, MGame.Instance.Ball },
            { MGame.EditMode.BALL_DEFAULT, MGame.Instance.BallDefault },
            { MGame.EditMode.BALL_CIRCLE, MGame.Instance.BallCircle },
            { MGame.EditMode.COIN, MGame.Instance.Coin },
            { MGame.EditMode.GRAY_KEY, MGame.Instance.GrayKey },
            { MGame.EditMode.RED_KEY, MGame.Instance.RedKey },
            { MGame.EditMode.GREEN_KEY, MGame.Instance.GreenKey },
            { MGame.EditMode.BLUE_KEY, MGame.Instance.BlueKey },
            { MGame.EditMode.YELLOW_KEY, MGame.Instance.YellowKey }
        };
        return prefabs[mode];
    }

    public static string GetUIString(this MGame.EditMode mode)
    {
        return new string[]{
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
            "Key - Yellow",
        }[(int)mode];
    }

    public static bool IsFieldType(this MGame.EditMode mode)
    {
        // list of all field types
        List<MGame.EditMode> fieldModes = new()
        {
            MGame.EditMode.WALL_FIELD,
            MGame.EditMode.START_FIELD,
            MGame.EditMode.GOAL_FIELD,
            MGame.EditMode.CHECKPOINT_FIELD,
            MGame.EditMode.ONE_WAY_FIELD,
            MGame.EditMode.CONVEYOR,
            MGame.EditMode.WATER,            
            MGame.EditMode.ICE,
            MGame.EditMode.VOID,
            MGame.EditMode.GRAY_KEY_DOOR_FIELD,
            MGame.EditMode.RED_KEY_DOOR_FIELD,
            MGame.EditMode.GREEN_KEY_DOOR_FIELD,
            MGame.EditMode.BLUE_KEY_DOOR_FIELD,
            MGame.EditMode.YELLOW_KEY_DOOR_FIELD
        };

        return fieldModes.Contains(mode);
    }
}