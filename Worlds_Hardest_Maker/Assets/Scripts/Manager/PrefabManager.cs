using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    [FormerlySerializedAs("wallField")] [Header("Fields")]
    public GameObject WallField;

    [FormerlySerializedAs("startField")] public GameObject StartField;
    [FormerlySerializedAs("goalField")] public GameObject GoalField;

    [FormerlySerializedAs("checkpointField")]
    public GameObject CheckpointField;

    [FormerlySerializedAs("oneWayField")] public GameObject OneWayField;
    [FormerlySerializedAs("conveyor")] public GameObject Conveyor;
    [FormerlySerializedAs("water")] public GameObject Water;
    [FormerlySerializedAs("ice")] public GameObject Ice;
    [FormerlySerializedAs("void")] public GameObject Void;

    [FormerlySerializedAs("grayKeyDoorField")]
    public GameObject GrayKeyDoorField;

    [FormerlySerializedAs("redKeyDoorField")]
    public GameObject RedKeyDoorField;

    [FormerlySerializedAs("greenKeyDoorField")]
    public GameObject GreenKeyDoorField;

    [FormerlySerializedAs("blueKeyDoorField")]
    public GameObject BlueKeyDoorField;

    [FormerlySerializedAs("yellowKeyDoorField")]
    public GameObject YellowKeyDoorField;

    [FormerlySerializedAs("player")] [Space] [Header("Entities")]
    public GameObject Player;

    [FormerlySerializedAs("anchor")] public GameObject Anchor;
    [FormerlySerializedAs("ball")] public GameObject Ball;
    [FormerlySerializedAs("ballDefault")] public GameObject BallDefault;
    [FormerlySerializedAs("ballCircle")] public GameObject BallCircle;
    [FormerlySerializedAs("coin")] public GameObject Coin;
    [FormerlySerializedAs("grayKey")] public GameObject GrayKey;
    [FormerlySerializedAs("redKey")] public GameObject RedKey;
    [FormerlySerializedAs("greenKey")] public GameObject GreenKey;
    [FormerlySerializedAs("blueKey")] public GameObject BlueKey;
    [FormerlySerializedAs("yellowKey")] public GameObject YellowKey;

    [FormerlySerializedAs("dropdownOptionPrefab")] [Space] [Header("UI")]
    public GameObject DropdownOptionPrefab;

    [FormerlySerializedAs("checkboxOptionPrefab")]
    public GameObject CheckboxOptionPrefab;

    [FormerlySerializedAs("sliderOptionPrefab")]
    public GameObject SliderOptionPrefab;

    [FormerlySerializedAs("numberInputOptionPrefab")]
    public GameObject NumberInputOptionPrefab;

    [FormerlySerializedAs("headerOptionPrefab")]
    public GameObject HeaderOptionPrefab;

    [FormerlySerializedAs("spaceOptionPrefab")]
    public GameObject SpaceOptionPrefab;

    [FormerlySerializedAs("fillPreview")] public GameObject FillPreview;
    [FormerlySerializedAs("tooltip")] public GameObject Tooltip;
    [Space] [Header("Anchor Blocks")] public GameObject AnchorConnector;
    public GameObject GoToBlockPrefab;
    public GameObject MoveToBlockPrefab;
    public GameObject RotateBlockPrefab;
    public GameObject StartRotatingBlockPrefab;
    public GameObject StopRotatingBlockPrefab;
    public GameObject MoveAndRotateBlockPrefab;
    public GameObject SetAngularSpeedBlockPrefab;
    public GameObject SetSpeedBlockPrefab;
    public GameObject SetEaseBlockPrefab;
    public GameObject WaitBlockPrefab;


    private void OnEnable()
    {
        if (Instance == null) Instance = this;
    }
}