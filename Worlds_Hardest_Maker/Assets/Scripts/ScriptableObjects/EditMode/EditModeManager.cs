using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using NaughtyAttributes;
using UnityEngine;

public class EditModeManager : MonoBehaviour
{
    public static EditModeManager Instance { get; private set; }
    
    [InitializationField] [Required] public DeleteMode DeleteMode;
    [InitializationField] [Required] public FieldMode AnchorPlatformMode;
    [InitializationField] [Required] public FieldMode WallMode;
    [InitializationField] [Required] public FieldMode StartMode;
    [InitializationField] [Required] public FieldMode GoalMode;
    [InitializationField] [Required] public FieldMode CheckpointMode;
    [InitializationField] [Required] public FieldMode VoidMode;
    [InitializationField] [Required] public FieldMode OneWayMode;
    [InitializationField] [Required] public FieldMode ConveyorMode;
    [InitializationField] [Required] public FieldMode WaterMode;
    [InitializationField] [Required] public FieldMode IceMode;
    [InitializationField] [Required] public EntityMode PlayerMode;
    [InitializationField] [Required] public EntityMode AnchorMode;
    [InitializationField] [Required] public EntityMode BallMode;
    [InitializationField] [Required] public EntityMode CoinMode;
    [InitializationField] [Required] public KeyMode GrayKeyMode;
    [InitializationField] [Required] public KeyMode RedKeyMode;
    [InitializationField] [Required] public KeyMode GreenKeyMode;
    [InitializationField] [Required] public KeyMode BlueKeyMode;
    [InitializationField] [Required] public KeyMode YellowKeyMode;
    [InitializationField] [Required] public KeyDoorMode GrayKeyDoorMode;
    [InitializationField] [Required] public KeyDoorMode RedKeyDoorMode;
    [InitializationField] [Required] public KeyDoorMode GreenKeyDoorMode;
    [InitializationField] [Required] public KeyDoorMode BlueKeyDoorMode;
    [InitializationField] [Required] public KeyDoorMode YellowKeyDoorMode;
    
    public static DeleteMode Delete => Instance.DeleteMode;
    public static FieldMode AnchorFloor => Instance.AnchorPlatformMode;
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
    public static EntityMode Ball => Instance.BallMode;
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
            AnchorFloor,
            Wall,
            Start, Goal, Checkpoint,
            Void,
            OneWay,
            Conveyor,
            Water, Ice,
            Player,
            Anchor, Ball,
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