using UnityEngine;

[ExecuteAlways]
public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    // TODO transfer prefabs from game manager to here

    [Header("Fields")] public GameObject WallField;
    public GameObject StartField;
    public GameObject GoalField;
    public GameObject CheckpointField;
    public GameObject OneWayField;
    public GameObject Conveyor;
    public GameObject Water;
    public GameObject Ice;
    public GameObject Void;
    public GameObject GrayKeyDoorField;
    public GameObject RedKeyDoorField;
    public GameObject GreenKeyDoorField;
    public GameObject BlueKeyDoorField;
    public GameObject YellowKeyDoorField;
    [Space] [Header("Entities")] public GameObject Player;
    public GameObject Anchor;
    public GameObject Ball;
    public GameObject BallDefault;
    public GameObject BallCircle;
    public GameObject Coin;
    public GameObject GrayKey;
    public GameObject RedKey;
    public GameObject GreenKey;
    public GameObject BlueKey;
    public GameObject YellowKey;
    [Space] [Header("UI")] public GameObject DropdownOptionPrefab;
    public GameObject CheckboxOptionPrefab;
    public GameObject SliderOptionPrefab;
    public GameObject NumberInputOptionPrefab;
    public GameObject HeaderOptionPrefab;
    public GameObject SpaceOptionPrefab;
    public GameObject FillPreview;
    public GameObject Tooltip;

    private void OnEnable()
    {
        if (Instance == null) Instance = this;
    }
}