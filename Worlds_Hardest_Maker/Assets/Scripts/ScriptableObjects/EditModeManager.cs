using System;
using System.Collections.Generic;
using System.Linq;
using AssetUsageDetectorNamespace;
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
    [InitializationField] [MustBeAssigned] public FieldMode OneWayMode;
    [InitializationField] [MustBeAssigned] public FieldMode ConveyorMode;
    [InitializationField] [MustBeAssigned] public FieldMode WaterMode;
    [InitializationField] [MustBeAssigned] public FieldMode IceMode;
    [InitializationField] [MustBeAssigned] public FieldMode VoidMode;
    [InitializationField] [MustBeAssigned] public KeydoorMode GrayKeyDoorMode;
    [InitializationField] [MustBeAssigned] public KeydoorMode RedKeyDoorMode;
    [InitializationField] [MustBeAssigned] public KeydoorMode GreenKeyDoorMode;
    [InitializationField] [MustBeAssigned] public KeydoorMode BlueKeyDoorMode;
    [InitializationField] [MustBeAssigned] public KeydoorMode YellowKeyDoorMode;
    [InitializationField] [MustBeAssigned] public EntityMode PlayerMode;
    [InitializationField] [MustBeAssigned] public EntityMode AnchorMode;
    [InitializationField] [MustBeAssigned] public EntityMode AnchorBallMode;
    [InitializationField] [MustBeAssigned] public EntityMode CoinMode;
    [InitializationField] [MustBeAssigned] public KeyMode GrayKeyMode;
    [InitializationField] [MustBeAssigned] public KeyMode RedKeyMode;
    [InitializationField] [MustBeAssigned] public KeyMode GreenKeyMode;
    [InitializationField] [MustBeAssigned] public KeyMode BlueKeyMode;
    [InitializationField] [MustBeAssigned] public KeyMode YellowKeyMode;
    
    public static DeleteMode Delete => Instance.DeleteMode;
    public static FieldMode Wall => Instance.WallMode;
    public static FieldMode Start => Instance.StartMode;
    public static FieldMode Goal => Instance.GoalMode;
    public static FieldMode Checkpoint => Instance.CheckpointMode;
    public static FieldMode OneWay => Instance.OneWayMode;
    public static FieldMode Conveyor => Instance.ConveyorMode;
    public static FieldMode Water => Instance.WaterMode;
    public static FieldMode Ice => Instance.IceMode;
    public static FieldMode Void => Instance.VoidMode;
    public static KeydoorMode GrayKeyDoor => Instance.GrayKeyDoorMode;
    public static KeydoorMode RedKeyDoor => Instance.RedKeyDoorMode;
    public static KeydoorMode GreenKeyDoor => Instance.GreenKeyDoorMode;
    public static KeydoorMode BlueKeyDoor => Instance.BlueKeyDoorMode;
    public static KeydoorMode YellowKeyDoor => Instance.YellowKeyDoorMode;
    public static EntityMode Player => Instance.PlayerMode;
    public static EntityMode Anchor => Instance.AnchorMode;
    public static EntityMode AnchorBall => Instance.AnchorBallMode;
    public static EntityMode Coin => Instance.CoinMode;
    public static KeyMode GrayKey => Instance.GrayKeyMode;
    public static KeyMode RedKey => Instance.RedKeyMode;
    public static KeyMode GreenKey => Instance.GreenKeyMode;
    public static KeyMode BlueKey => Instance.BlueKeyMode;
    public static KeyMode YellowKey => Instance.YellowKeyMode;

    public List<EditMode> AllEditModes { get; private set; }
    public List<FieldMode> AllFieldModes { get; private set; }
    public List<FieldMode> AllPlayerStartFieldModes { get; private set; }

    private void Awake()
    {
        if (Instance != null) return;
        
        Instance = this;
        AllEditModes = new()
        {
            Delete,
            Wall,
            Start, Goal, Checkpoint,
            OneWay,
            Conveyor,
            Water, Ice,
            GrayKeyDoor, RedKeyDoor, GreenKeyDoor, BlueKeyDoor, YellowKeyDoor,
            Player,
            Anchor, AnchorBall,
            Coin,
            GrayKey, RedKey, GreenKey, BlueKey, YellowKey,
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
