using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// extension for GameManager.EditMode methods and readability
/// </summary>
public static class EditModeExtension
{
    public static FollowMouse.WorldPosition GetWorldPosition(this GameManager.EditMode mode)
    {
        if (mode.IsFieldType() || mode == GameManager.EditMode.DELETE_FIELD) return FollowMouse.WorldPosition.MATRIX;
        return FollowMouse.WorldPosition.GRID;
    }

    public static GameObject GetPrefab(this GameManager.EditMode mode)
    {
        Dictionary<GameManager.EditMode, GameObject> prefabs = new()
        {
            { GameManager.EditMode.WALL_FIELD, GameManager.Instance.WallField },
            { GameManager.EditMode.START_FIELD, GameManager.Instance.StartField },
            { GameManager.EditMode.GOAL_FIELD, GameManager.Instance.GoalField },
            { GameManager.EditMode.START_AND_GOAL_FIELD, GameManager.Instance.StartAndGoalField },
            { GameManager.EditMode.CHECKPOINT_FIELD, GameManager.Instance.CheckpointField },
            { GameManager.EditMode.ONE_WAY_FIELD, GameManager.Instance.OneWayField },
            { GameManager.EditMode.WATER, GameManager.Instance.Water },
            { GameManager.EditMode.ICE, GameManager.Instance.Ice },
            { GameManager.EditMode.VOID, GameManager.Instance.Void },
            { GameManager.EditMode.GRAY_KEY_DOOR_FIELD, GameManager.Instance.GrayKeyDoorField },
            { GameManager.EditMode.RED_KEY_DOOR_FIELD, GameManager.Instance.RedKeyDoorField },
            { GameManager.EditMode.GREEN_KEY_DOOR_FIELD, GameManager.Instance.GreenKeyDoorField },
            { GameManager.EditMode.BLUE_KEY_DOOR_FIELD, GameManager.Instance.BlueKeyDoorField },
            { GameManager.EditMode.YELLOW_KEY_DOOR_FIELD, GameManager.Instance.YellowKeyDoorField },
            { GameManager.EditMode.PLAYER, GameManager.Instance.Player },
            { GameManager.EditMode.ANCHOR, GameManager.Instance.Anchor },
            { GameManager.EditMode.BALL, GameManager.Instance.Ball },
            { GameManager.EditMode.BALL_DEFAULT, GameManager.Instance.BallDefault },
            { GameManager.EditMode.BALL_CIRCLE, GameManager.Instance.BallCircle },
            { GameManager.EditMode.COIN, GameManager.Instance.Coin },
            { GameManager.EditMode.GRAY_KEY, GameManager.Instance.GrayKey },
            { GameManager.EditMode.RED_KEY, GameManager.Instance.RedKey },
            { GameManager.EditMode.GREEN_KEY, GameManager.Instance.GreenKey },
            { GameManager.EditMode.BLUE_KEY, GameManager.Instance.BlueKey },
            { GameManager.EditMode.YELLOW_KEY, GameManager.Instance.YellowKey }
        };
        return prefabs[mode];
    }

    public static string GetUIString(this GameManager.EditMode mode)
    {
        return new string[]{
            "Delete",
            "Wall Field",
            "Start Field",
            "Goal Field",
            "Start/Goal Field",
            "Checkpoint Field",
            "One Way Gate",
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

    public static bool IsFieldType(this GameManager.EditMode mode)
    {
        // list of all field types
        List<GameManager.EditMode> fieldModes = new()
        {
            GameManager.EditMode.WALL_FIELD,
            GameManager.EditMode.START_FIELD,
            GameManager.EditMode.GOAL_FIELD,
            GameManager.EditMode.START_AND_GOAL_FIELD,
            GameManager.EditMode.CHECKPOINT_FIELD,
            GameManager.EditMode.ONE_WAY_FIELD,
            GameManager.EditMode.WATER,            
            GameManager.EditMode.ICE,
            GameManager.EditMode.VOID,
            GameManager.EditMode.GRAY_KEY_DOOR_FIELD,
            GameManager.EditMode.RED_KEY_DOOR_FIELD,
            GameManager.EditMode.GREEN_KEY_DOOR_FIELD,
            GameManager.EditMode.BLUE_KEY_DOOR_FIELD,
            GameManager.EditMode.YELLOW_KEY_DOOR_FIELD
        };

        return fieldModes.Contains(mode);
    }
}