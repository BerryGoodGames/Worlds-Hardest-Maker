// using System.Collections.Generic;
// using MyBox;
// using UnityEngine;
//
// public class FieldModeManager : MonoBehaviour
// {
//     public static FieldModeManager Instance { get; private set; }
//     
//     [InitializationField] [MustBeAssigned] public FieldMode WallMode;
//     [InitializationField] [MustBeAssigned] public FieldMode StartMode;
//     [InitializationField] [MustBeAssigned] public FieldMode GoalMode;
//     [InitializationField] [MustBeAssigned] public FieldMode CheckpointMode;
//     [InitializationField] [MustBeAssigned] public FieldMode OneWayMode;
//     [InitializationField] [MustBeAssigned] public FieldMode ConveyorMode;
//     [InitializationField] [MustBeAssigned] public FieldMode WaterMode;
//     [InitializationField] [MustBeAssigned] public FieldMode IceMode;
//     [InitializationField] [MustBeAssigned] public FieldMode VoidMode;
//     [InitializationField] [MustBeAssigned] public FieldMode GrayKeyDoorMode;
//     [InitializationField] [MustBeAssigned] public FieldMode RedKeyDoorMode;
//     [InitializationField] [MustBeAssigned] public FieldMode GreenKeyDoorMode;
//     [InitializationField] [MustBeAssigned] public FieldMode BlueKeyDoorMode;
//     [InitializationField] [MustBeAssigned] public FieldMode YellowKeyDoorMode;
//     
//     public static FieldMode Wall => Instance.WallMode;
//     public static FieldMode Start => Instance.StartMode;
//     public static FieldMode Goal => Instance.GoalMode;
//     public static FieldMode Checkpoint => Instance.CheckpointMode;
//     public static FieldMode OneWay => Instance.OneWayMode;
//     public static FieldMode Conveyor => Instance.ConveyorMode;
//     public static FieldMode Water => Instance.WaterMode;
//     public static FieldMode Ice => Instance.IceMode;
//     public static FieldMode Void => Instance.VoidMode;
//     public static FieldMode GrayKeyDoor => Instance.GrayKeyDoorMode;
//     public static FieldMode RedKeyDoor => Instance.RedKeyDoorMode;
//     public static FieldMode GreenKeyDoor => Instance.GreenKeyDoorMode;
//     public static FieldMode BlueKeyDoor => Instance.BlueKeyDoorMode;
//     public static FieldMode YellowKeyDoor => Instance.YellowKeyDoorMode;
//
//     public static List<FieldMode> AllFieldModes => new()
//     {
//         Wall,
//         Start,
//         Goal,
//         Checkpoint,
//         OneWay,
//         Conveyor,
//         Water,
//         Ice,
//         RedKeyDoor,
//         GreenKeyDoor,
//         BlueKeyDoor,
//         YellowKeyDoor,
//     };
//     
//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//     }
// }
