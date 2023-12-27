using MyBox;
using UnityEngine;

public class EditModeManager : MonoBehaviour
{
    public static EditModeManager Instance { get; private set; }
    
    [InitializationField] [MustBeAssigned] public EditMode DeleteMode;
    [InitializationField] [MustBeAssigned] public EditMode WallMode;
    [InitializationField] [MustBeAssigned] public EditMode StartMode;
    [InitializationField] [MustBeAssigned] public EditMode GoalMode;
    [InitializationField] [MustBeAssigned] public EditMode CheckpointMode;
    [InitializationField] [MustBeAssigned] public EditMode OneWayMode;
    [InitializationField] [MustBeAssigned] public EditMode ConveyorMode;
    [InitializationField] [MustBeAssigned] public EditMode WaterMode;
    [InitializationField] [MustBeAssigned] public EditMode IceMode;
    [InitializationField] [MustBeAssigned] public EditMode VoidMode;
    [InitializationField] [MustBeAssigned] public EditMode GrayKeyDoorMode;
    [InitializationField] [MustBeAssigned] public EditMode RedKeyDoorMode;
    [InitializationField] [MustBeAssigned] public EditMode GreenKeyDoorMode;
    [InitializationField] [MustBeAssigned] public EditMode BlueKeyDoorMode;
    [InitializationField] [MustBeAssigned] public EditMode YellowKeyDoorMode;
    [InitializationField] [MustBeAssigned] public EditMode PlayerMode;
    [InitializationField] [MustBeAssigned] public EditMode AnchorMode;
    [InitializationField] [MustBeAssigned] public EditMode AnchorBallMode;
    [InitializationField] [MustBeAssigned] public EditMode CoinMode;
    [InitializationField] [MustBeAssigned] public EditMode GrayKeyMode;
    [InitializationField] [MustBeAssigned] public EditMode RedKeyMode;
    [InitializationField] [MustBeAssigned] public EditMode GreenKeyMode;
    [InitializationField] [MustBeAssigned] public EditMode BlueKeyMode;
    [InitializationField] [MustBeAssigned] public EditMode YellowKeyMode;
    
    public static EditMode Delete => Instance.DeleteMode;
    public static EditMode Wall => Instance.WallMode;
    public static EditMode Start => Instance.StartMode;
    public static EditMode Goal => Instance.GoalMode;
    public static EditMode Checkpoint => Instance.CheckpointMode;
    public static EditMode OneWay => Instance.OneWayMode;
    public static EditMode Conveyor => Instance.ConveyorMode;
    public static EditMode Water => Instance.WaterMode;
    public static EditMode Ice => Instance.IceMode;
    public static EditMode Void => Instance.VoidMode;
    public static EditMode GrayKeyDoor => Instance.GrayKeyDoorMode;
    public static EditMode RedKeyDoor => Instance.RedKeyDoorMode;
    public static EditMode GreenKeyDoor => Instance.GreenKeyDoorMode;
    public static EditMode BlueKeyDoor => Instance.BlueKeyDoorMode;
    public static EditMode YellowKeyDoor => Instance.YellowKeyDoorMode;
    public static EditMode Player => Instance.PlayerMode;
    public static EditMode Anchor => Instance.AnchorMode;
    public static EditMode AnchorBall => Instance.AnchorBallMode;
    public static EditMode Coin => Instance.CoinMode;
    public static EditMode GrayKey => Instance.GrayKeyMode;
    public static EditMode RedKey => Instance.RedKeyMode;
    public static EditMode GreenKey => Instance.GreenKeyMode;
    public static EditMode BlueKey => Instance.BlueKeyMode;
    public static EditMode YellowKey => Instance.YellowKeyMode;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
