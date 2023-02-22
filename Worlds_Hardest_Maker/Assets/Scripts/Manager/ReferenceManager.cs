using UnityEngine;
using UnityEngine.Serialization;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance { get; private set; }

    [FormerlySerializedAs("manager")] [Header("Objects")]
    public GameObject Manager;

    [FormerlySerializedAs("canvas")] public Canvas Canvas;

    [FormerlySerializedAs("tooltipCanvas")]
    public GameObject TooltipCanvas;

    [FormerlySerializedAs("menu")] public GameObject Menu;
    [FormerlySerializedAs("playButton")] public GameObject PlayButton;

    [FormerlySerializedAs("placementPreview")]
    public GameObject PlacementPreview;

    [FormerlySerializedAs("levelSettingsPanel")]
    public GameObject LevelSettingsPanel;

    public PanelTween LevelSettingsButtonPanelTween;
    public PanelTween LevelSettingsPanelTween;

    public PanelTween AnchorEditorButtonPanelTween;
    public PanelTween AnchorEditorPanelTween;

    [FormerlySerializedAs("ballWindows")] public GameObject BallWindows;

    [FormerlySerializedAs("playerSpawner")]
    public PlayerSpawner PlayerSpawner;

    [FormerlySerializedAs("toolbarContainer")] [Space] [Header("Containers")]
    public Transform ToolbarContainer;

    [FormerlySerializedAs("sliderContainer")]
    public Transform SliderContainer;

    [FormerlySerializedAs("nameTagContainer")]
    public Transform NameTagContainer;

    [FormerlySerializedAs("drawContainer")]
    public Transform DrawContainer;

    [FormerlySerializedAs("selectionOutlineContainer")]
    public Transform SelectionOutlineContainer;

    [FormerlySerializedAs("fillPreviewContainer")]
    public Transform FillPreviewContainer;

    [FormerlySerializedAs("playerContainer")]
    public Transform PlayerContainer;

    [FormerlySerializedAs("anchorContainer")]
    public Transform AnchorContainer;

    [FormerlySerializedAs("ballDefaultContainer")]
    public Transform BallDefaultContainer;

    [FormerlySerializedAs("ballCircleContainer")]
    public Transform BallCircleContainer;

    [FormerlySerializedAs("coinContainer")]
    public Transform CoinContainer;

    [FormerlySerializedAs("keyContainer")] public Transform KeyContainer;

    [FormerlySerializedAs("fieldContainer")]
    public Transform FieldContainer;

    public Transform AnchorBlockContainer;

    public Transform AnchorBlockStringContainer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}