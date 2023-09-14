using MyBox;
using UnityEngine;

[ExecuteAlways]
public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    #region Fields

    [Foldout("Fields")] public GameObject WallField;

    [Foldout("Fields")] public GameObject StartField;
    [Foldout("Fields")] public GameObject GoalField;
    [Foldout("Fields")] public GameObject CheckpointField;

    [Foldout("Fields")] public GameObject OneWayField;
    [Foldout("Fields")] public GameObject Conveyor;
    [Foldout("Fields")] public GameObject Water;
    [Foldout("Fields")] public GameObject Ice;
    [Foldout("Fields")] public GameObject Void;

    [Foldout("Fields")] public GameObject GrayKeyDoorField;
    [Foldout("Fields")] public GameObject RedKeyDoorField;
    [Foldout("Fields")] public GameObject GreenKeyDoorField;
    [Foldout("Fields")] public GameObject BlueKeyDoorField;
    [Foldout("Fields")] public GameObject YellowKeyDoorField;

    #endregion

    #region Entities

    [Foldout("Entities")] public GameObject Player;

    [Foldout("Entities")] public GameObject Anchor;
    [Foldout("Entities")] public GameObject AnchorBall;
    [Foldout("Entities")] public GameObject BallDefault;
    [Foldout("Entities")] public GameObject BallCircle;

    [Foldout("Entities")] public GameObject Coin;

    [Foldout("Entities")] public GameObject GrayKey;
    [Foldout("Entities")] public GameObject RedKey;
    [Foldout("Entities")] public GameObject GreenKey;
    [Foldout("Entities")] public GameObject BlueKey;
    [Foldout("Entities")] public GameObject YellowKey;

    #endregion

    #region UI

    [Foldout("UI")] public GameObject DropdownOptionPrefab;
    [Foldout("UI")] public GameObject CheckboxOptionPrefab;
    [Foldout("UI")] public GameObject SliderOptionPrefab;
    [Foldout("UI")] public GameObject NumberInputOptionPrefab;
    [Foldout("UI")] public GameObject HeaderOptionPrefab;
    [Foldout("UI")] public GameObject SpaceOptionPrefab;
    [Foldout("UI")] public GameObject GlowPrefab;

    [Foldout("UI")] public GameObject FillPreview;
    [Foldout("UI")] public GameObject Tooltip;

    #endregion

    #region Anchor blocks

    [Foldout("Anchor blocks")] public GameObject AnchorConnector;
    [Foldout("Anchor blocks")] public GameObject GoToBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject MoveBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject TeleportBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject RotateBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject StartRotatingBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject StopRotatingBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject MoveAndRotateBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject SetRotationSpeedBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject SetDirectionBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject SetSpeedBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject SetEaseBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject WaitBlockPrefab;

    #endregion

    private void OnEnable()
    {
        if (Instance == null) Instance = this;
    }
}