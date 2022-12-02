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
            { GameManager.EditMode.WALL_FIELD, PrefabManager.Instance.WallField },
            { GameManager.EditMode.START_FIELD, PrefabManager.Instance.StartField },
            { GameManager.EditMode.GOAL_FIELD, PrefabManager.Instance.GoalField },
            { GameManager.EditMode.CHECKPOINT_FIELD, PrefabManager.Instance.CheckpointField },
            { GameManager.EditMode.ONE_WAY_FIELD, PrefabManager.Instance.OneWayField },
            { GameManager.EditMode.CONVEYOR, PrefabManager.Instance.Conveyor },
            { GameManager.EditMode.WATER, PrefabManager.Instance.Water },
            { GameManager.EditMode.ICE, PrefabManager.Instance.Ice },
            { GameManager.EditMode.VOID, PrefabManager.Instance.Void },
            { GameManager.EditMode.GRAY_KEY_DOOR_FIELD, PrefabManager.Instance.GrayKeyDoorField },
            { GameManager.EditMode.RED_KEY_DOOR_FIELD, PrefabManager.Instance.RedKeyDoorField },
            { GameManager.EditMode.GREEN_KEY_DOOR_FIELD, PrefabManager.Instance.GreenKeyDoorField },
            { GameManager.EditMode.BLUE_KEY_DOOR_FIELD, PrefabManager.Instance.BlueKeyDoorField },
            { GameManager.EditMode.YELLOW_KEY_DOOR_FIELD, PrefabManager.Instance.YellowKeyDoorField },
            { GameManager.EditMode.PLAYER, PrefabManager.Instance.Player },
            { GameManager.EditMode.ANCHOR, PrefabManager.Instance.Anchor },
            { GameManager.EditMode.BALL, PrefabManager.Instance.Ball },
            { GameManager.EditMode.BALL_DEFAULT, PrefabManager.Instance.BallDefault },
            { GameManager.EditMode.BALL_CIRCLE, PrefabManager.Instance.BallCircle },
            { GameManager.EditMode.COIN, PrefabManager.Instance.Coin },
            { GameManager.EditMode.GRAY_KEY, PrefabManager.Instance.GrayKey },
            { GameManager.EditMode.RED_KEY, PrefabManager.Instance.RedKey },
            { GameManager.EditMode.GREEN_KEY, PrefabManager.Instance.GreenKey },
            { GameManager.EditMode.BLUE_KEY, PrefabManager.Instance.BlueKey },
            { GameManager.EditMode.YELLOW_KEY, PrefabManager.Instance.YellowKey }
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

    public static bool IsFieldType(this GameManager.EditMode mode)
    {
        // list of all field types
        List<GameManager.EditMode> fieldModes = new()
        {
            GameManager.EditMode.WALL_FIELD,
            GameManager.EditMode.START_FIELD,
            GameManager.EditMode.GOAL_FIELD,
            GameManager.EditMode.CHECKPOINT_FIELD,
            GameManager.EditMode.ONE_WAY_FIELD,
            GameManager.EditMode.CONVEYOR,
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