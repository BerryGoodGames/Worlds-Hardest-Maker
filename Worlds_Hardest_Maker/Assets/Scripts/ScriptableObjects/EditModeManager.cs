using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

public class EditModeManager : MonoBehaviour
{
    public static EditModeManager Instance { get; private set; }

    [InitializationField] [MustBeAssigned] public DeleteMode DeleteMode;
    [InitializationField] [MustBeAssigned] public FieldMode WallMode;
    [InitializationField] [MustBeAssigned] public FieldMode StartMode;
    [InitializationField] [MustBeAssigned] public FieldMode GoalMode;
    [InitializationField] [MustBeAssigned] public FieldMode CheckpointMode;
    [InitializationField] [MustBeAssigned] public FieldMode VoidMode;
    [InitializationField] [MustBeAssigned] public FieldMode OneWayMode;
    [InitializationField] [MustBeAssigned] public FieldMode ConveyorMode;
    [InitializationField] [MustBeAssigned] public FieldMode WaterMode;
    [InitializationField] [MustBeAssigned] public FieldMode IceMode;
    [InitializationField] [MustBeAssigned] public EntityMode PlayerMode;
    [InitializationField] [MustBeAssigned] public EntityMode AnchorMode;
    [InitializationField] [MustBeAssigned] public EntityMode AnchorBallMode;
    [InitializationField] [MustBeAssigned] public EntityMode CoinMode;
    [InitializationField] [MustBeAssigned] public KeyMode GrayKeyMode;
    [InitializationField] [MustBeAssigned] public KeyMode RedKeyMode;
    [InitializationField] [MustBeAssigned] public KeyMode GreenKeyMode;
    [InitializationField] [MustBeAssigned] public KeyMode BlueKeyMode;
    [InitializationField] [MustBeAssigned] public KeyMode YellowKeyMode;
    [InitializationField] [MustBeAssigned] public KeyDoorMode GrayKeyDoorMode;
    [InitializationField] [MustBeAssigned] public KeyDoorMode RedKeyDoorMode;
    [InitializationField] [MustBeAssigned] public KeyDoorMode GreenKeyDoorMode;
    [InitializationField] [MustBeAssigned] public KeyDoorMode BlueKeyDoorMode;
    [InitializationField] [MustBeAssigned] public KeyDoorMode YellowKeyDoorMode;

    public static DeleteMode Delete => Instance.DeleteMode;
    public static FieldMode Wall => Instance.WallMode;
    public static FieldMode Start => Instance.StartMode;
    public static FieldMode Goal => Instance.GoalMode;
    public static FieldMode Checkpoint => Instance.CheckpointMode;
    public static FieldMode Void => Instance.VoidMode;
    public static FieldMode OneWay => Instance.OneWayMode;
    public static FieldMode Conveyor => Instance.ConveyorMode;
    public static FieldMode Water => Instance.WaterMode;
    public static FieldMode Ice => Instance.IceMode;
    public static EntityMode Player => Instance.PlayerMode;
    public static EntityMode Anchor => Instance.AnchorMode;
    public static EntityMode AnchorBall => Instance.AnchorBallMode;
    public static EntityMode Coin => Instance.CoinMode;
    public static KeyMode GrayKey => Instance.GrayKeyMode;
    public static KeyMode RedKey => Instance.RedKeyMode;
    public static KeyMode GreenKey => Instance.GreenKeyMode;
    public static KeyMode BlueKey => Instance.BlueKeyMode;
    public static KeyMode YellowKey => Instance.YellowKeyMode;
    public static KeyDoorMode GrayKeyDoor => Instance.GrayKeyDoorMode;
    public static KeyDoorMode RedKeyDoor => Instance.RedKeyDoorMode;
    public static KeyDoorMode GreenKeyDoor => Instance.GreenKeyDoorMode;
    public static KeyDoorMode BlueKeyDoor => Instance.BlueKeyDoorMode;
    public static KeyDoorMode YellowKeyDoor => Instance.YellowKeyDoorMode;

    public List<EditMode> AllEditModes { get; private set; }
    public List<FieldMode> AllFieldModes { get; private set; }
    public List<FieldMode> AllPlayerStartFieldModes { get; private set; }

    public static EditMode GetEditMode(string editModeName)
    {
        try
        {
            EditMode editMode = Instance.AllEditModes.First(e => e.name == editModeName);

            if (editMode == null) throw new();

            return editMode;
        }
        catch (Exception)
        {
            Console.WriteLine($"Edit mode with name \"{editModeName}\" was not found");
            throw;
        }
    }

    public static FieldMode GetFieldMode(string fieldModeName)
    {
        try
        {
            FieldMode fieldMode = Instance.AllFieldModes.First(e => e.name == fieldModeName);

            if (fieldMode == null) throw new();

            return fieldMode;
        }
        catch (Exception)
        {
            Console.WriteLine($"Field mode with name \"{fieldModeName}\" was not found");
            throw;
        }
    }

    private void Awake()
    {
        if (Instance != null) return;

        Instance = this;
        AllEditModes = new()
        {
            Delete,
            Wall,
            Start, Goal, Checkpoint,
            Void,
            OneWay,
            Conveyor,
            Water, Ice,
            Player,
            Anchor, AnchorBall,
            Coin,
            GrayKey, RedKey, GreenKey, BlueKey, YellowKey,
            GrayKeyDoor, RedKeyDoor, GreenKeyDoor, BlueKeyDoor, YellowKeyDoor,
        };

        // cache AllFieldModes
        AllFieldModes = AllEditModes
            .Where(editMode => editMode.Attributes.IsField)
            .OfType<FieldMode>()
            .ToList();

        // cache AllPlayerStartFieldModes
        AllPlayerStartFieldModes = AllFieldModes
            .Where(fieldMode => fieldMode.IsStartFieldForPlayer)
            .ToList();
    }
}