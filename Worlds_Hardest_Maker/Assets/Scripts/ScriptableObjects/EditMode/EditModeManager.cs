using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

public class EditModeManager : MonoBehaviour
{
    public static EditModeManager Instance { get; private set; }
    
    [InitializationField] [MustBeAssigned] public DeleteMode DeleteMode;
    [InitializationField] [MustBeAssigned] public FieldMode AnchorPlatformMode;
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
    [InitializationField] [MustBeAssigned] public EntityMode BallMode;
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

    [SerializeField] private List<EditMode> allEditModes;
    private List<FieldMode> allFieldModes;
    private List<FieldMode> allPlayerStartFieldModes;

    public IReadOnlyList<EditMode> AllEditModes => allEditModes;
    public IReadOnlyList<FieldMode> AllFieldModes => allFieldModes;
    public IReadOnlyList<FieldMode> AllPlayerStartFieldModes => allPlayerStartFieldModes;
    
    public EditMode GetEditMode(string editModeName)
    {
        // TODO: do not use the asset name, instead tag
        EditMode editMode = allEditModes.First(e => e.name == editModeName);
        
        if (editMode == null)
        {
            throw new ArgumentOutOfRangeException(nameof(editModeName), $"No existing field mode with the name {editModeName}");
        }
        
        return editMode;
    }
    
    public FieldMode GetFieldMode(string fieldModeName)
    {
        FieldMode fieldMode = allFieldModes.FirstOrDefault(e => e.name == fieldModeName);
        
        if (fieldMode == null)
        {
            throw new ArgumentOutOfRangeException(nameof(fieldModeName), $"No existing field mode with the name {fieldModeName}");
        }
        
        return fieldMode;
    }
    
    private void Awake()
    {
        if (Instance != null) return;
        
        Instance = this;
        
        allFieldModes = allEditModes
            .OfType<FieldMode>()
            .ToList();
        
        allPlayerStartFieldModes = allFieldModes
            .Where(fieldMode => fieldMode.IsStartFieldForPlayer)
            .ToList();
    }
}