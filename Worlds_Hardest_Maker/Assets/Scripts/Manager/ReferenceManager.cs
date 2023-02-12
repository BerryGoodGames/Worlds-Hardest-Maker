using UnityEngine;
using UnityEngine.Serialization;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance { get; private set; }

    [FormerlySerializedAs("Manager")] [Header("Objects")]
    public GameObject manager;

    [FormerlySerializedAs("Canvas")] public GameObject canvas;

    [FormerlySerializedAs("TooltipCanvas")]
    public GameObject tooltipCanvas;

    [FormerlySerializedAs("Menu")] public GameObject menu;
    [FormerlySerializedAs("PlayButton")] public GameObject playButton;

    [FormerlySerializedAs("PlacementPreview")]
    public GameObject placementPreview;

    [FormerlySerializedAs("LevelSettingsPanel")]
    public GameObject levelSettingsPanel;

    public PanelTween LevelSettingsButtonPanelTween;
    public PanelTween LevelSettingsPanelTween;

    public PanelTween AnchorEditorButtonPanelTween;
    public PanelTween AnchorEditorPanelTween;

    [FormerlySerializedAs("BallWindows")] public GameObject ballWindows;

    [FormerlySerializedAs("PlayerSpawner")]
    public PlayerSpawner playerSpawner;

    [FormerlySerializedAs("ToolbarContainer")] [Space] [Header("Containers")]
    public Transform toolbarContainer;

    [FormerlySerializedAs("SliderContainer")]
    public Transform sliderContainer;

    [FormerlySerializedAs("NameTagContainer")]
    public Transform nameTagContainer;

    [FormerlySerializedAs("DrawContainer")]
    public Transform drawContainer;

    [FormerlySerializedAs("SelectionOutlineContainer")]
    public Transform selectionOutlineContainer;

    [FormerlySerializedAs("FillPreviewContainer")]
    public Transform fillPreviewContainer;

    [FormerlySerializedAs("PlayerContainer")]
    public Transform playerContainer;

    [FormerlySerializedAs("AnchorContainer")]
    public Transform anchorContainer;

    [FormerlySerializedAs("BallDefaultContainer")]
    public Transform ballDefaultContainer;

    [FormerlySerializedAs("BallCircleContainer")]
    public Transform ballCircleContainer;

    [FormerlySerializedAs("CoinContainer")]
    public Transform coinContainer;

    [FormerlySerializedAs("KeyContainer")] public Transform keyContainer;

    [FormerlySerializedAs("FieldContainer")]
    public Transform fieldContainer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}