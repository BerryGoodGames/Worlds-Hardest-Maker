using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    // TODO transfer prefabs from game manager to here

    [FormerlySerializedAs("WallField")] [Header("Fields")] public GameObject wallField;
    [FormerlySerializedAs("StartField")] public GameObject startField;
    [FormerlySerializedAs("GoalField")] public GameObject goalField;
    [FormerlySerializedAs("CheckpointField")] public GameObject checkpointField;
    [FormerlySerializedAs("OneWayField")] public GameObject oneWayField;
    [FormerlySerializedAs("Conveyor")] public GameObject conveyor;
    [FormerlySerializedAs("Water")] public GameObject water;
    [FormerlySerializedAs("Ice")] public GameObject ice;
    [FormerlySerializedAs("Void")] public GameObject @void;
    [FormerlySerializedAs("GrayKeyDoorField")] public GameObject grayKeyDoorField;
    [FormerlySerializedAs("RedKeyDoorField")] public GameObject redKeyDoorField;
    [FormerlySerializedAs("GreenKeyDoorField")] public GameObject greenKeyDoorField;
    [FormerlySerializedAs("BlueKeyDoorField")] public GameObject blueKeyDoorField;
    [FormerlySerializedAs("YellowKeyDoorField")] public GameObject yellowKeyDoorField;
    [FormerlySerializedAs("Player")] [Space] [Header("Entities")] public GameObject player;
    [FormerlySerializedAs("Anchor")] public GameObject anchor;
    [FormerlySerializedAs("Ball")] public GameObject ball;
    [FormerlySerializedAs("BallDefault")] public GameObject ballDefault;
    [FormerlySerializedAs("BallCircle")] public GameObject ballCircle;
    [FormerlySerializedAs("Coin")] public GameObject coin;
    [FormerlySerializedAs("GrayKey")] public GameObject grayKey;
    [FormerlySerializedAs("RedKey")] public GameObject redKey;
    [FormerlySerializedAs("GreenKey")] public GameObject greenKey;
    [FormerlySerializedAs("BlueKey")] public GameObject blueKey;
    [FormerlySerializedAs("YellowKey")] public GameObject yellowKey;
    [FormerlySerializedAs("DropdownOptionPrefab")] [Space] [Header("UI")] public GameObject dropdownOptionPrefab;
    [FormerlySerializedAs("CheckboxOptionPrefab")] public GameObject checkboxOptionPrefab;
    [FormerlySerializedAs("SliderOptionPrefab")] public GameObject sliderOptionPrefab;
    [FormerlySerializedAs("NumberInputOptionPrefab")] public GameObject numberInputOptionPrefab;
    [FormerlySerializedAs("HeaderOptionPrefab")] public GameObject headerOptionPrefab;
    [FormerlySerializedAs("SpaceOptionPrefab")] public GameObject spaceOptionPrefab;
    [FormerlySerializedAs("FillPreview")] public GameObject fillPreview;
    [FormerlySerializedAs("Tooltip")] public GameObject tooltip;

    private void OnEnable()
    {
        if (Instance == null) Instance = this;
    }
}