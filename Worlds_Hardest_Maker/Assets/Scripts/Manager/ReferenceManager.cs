using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance { get; private set; }

    #region Objects

    [Foldout("Objects")] public GameObject Manager;

    [Foldout("Objects")] public JumpToEntity MainCameraJumper;

    [Foldout("Objects")] public Canvas Canvas;

    [Foldout("Objects")] public GameObject TooltipCanvas;

    [Foldout("Objects")] public GameObject Menu;
    [Foldout("Objects")] public GameObject PlayButton;

    [Foldout("Objects")] public PreviewController PlacementPreview;

    [Foldout("Objects")] public BarTween ToolbarTween;
    [Foldout("Objects")] public BarTween InfobarEditTween;
    [Foldout("Objects")] public BarTween PlayButtonTween;

    [Foldout("Objects")] public PanelController LevelSettingsPanelController;
    [Foldout("Objects")] public PanelController AnchorPanelController;

    [Foldout("Objects")] public PlayerSpawner PlayerSpawner;

    #endregion

    #region Containers

    [Foldout("Containers")] public Transform ToolbarContainer;

    [Foldout("Containers")] public Transform SliderContainer;

    [Foldout("Containers")] public Transform NameTagContainer;

    [Foldout("Containers")] public Transform DrawContainer;

    [Foldout("Containers")] public Transform SelectionOutlineContainer;

    [Foldout("Containers")] public Transform FillPreviewContainer;

    [Foldout("Containers")] public Transform PlayerContainer;

    [Foldout("Containers")] public Transform AnchorContainer;

    [Foldout("Containers")] public AnchorBallFade AnchorBallContainer;

    [Foldout("Containers")] public Transform BallDefaultContainer;

    [Foldout("Containers")] public Transform BallCircleContainer;

    [Foldout("Containers")] public Transform CoinContainer;

    [Foldout("Containers")] public Transform KeyContainer;

    [Foldout("Containers")] public Transform FieldContainer;

    #endregion

    #region Anchor

    [Foldout("Anchor")] public PanelTween AnchorEditorButtonPanelTween;

    [Foldout("Anchor")] public PanelTween AnchorEditorPanelTween;

    [Foldout("Anchor")] public RectTransform AnchorBlockContainer;

    [Foldout("Anchor")] public RectTransform AnchorBlockChainContainer;

    [Foldout("Anchor")] public RectTransform AnchorBlockSourceContainer;

    [Foldout("Anchor")] public AnchorBlockQuickMenuController AnchorBlockQuickMenu;

    [Foldout("Anchor")] public ChainController MainChainController;

    [Foldout("Anchor")] public AnchorBlockFitter AnchorBlockFitter;

    [Foldout("Anchor")] public AnchorBlockConnectorController AnchorBlockConnectorController;

    [Foldout("Anchor")] public AnchorBlockPreviewController AnchorBlockPreview;

    [Foldout("Anchor")] public AnchorBlockPeriblockerController AnchorBlockPeriblocker;

    [Foldout("Anchor")] public AnchorCameraJumping AnchorCameraJumping;

    [Foldout("Anchor")] public AlphaTween AnchorNoAnchorSelectedScreen;

    [Foldout("Anchor")] public AlphaTween AnchorInPlayModeScreen;

    #endregion

    #region Materials

    [Foldout("Material")] public Material DashedLineMaterial;

    #endregion

    #region SettingInputs

    [Foldout("SettingInputs")] public TMP_Dropdown QualityDropdown;
    [Foldout("SettingInputs")] public TMP_Dropdown ResolutionDropdown;
    [Foldout("SettingInputs")] public Toggle FullscreenToggle;
    [Foldout("SettingInputs")] public Toggle OneColorToggle;
    [Foldout("SettingInputs")] public SyncInputToSlider ToolbarSizeSlider;
    [Foldout("SettingInputs")] public SyncInputToSlider InfobarSizeSlider;
    [Foldout("SettingInputs")] public SyncInputToSlider MusicSlider;
    [Foldout("SettingInputs")] public SyncInputToSlider SoundEffectSlider;

    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}