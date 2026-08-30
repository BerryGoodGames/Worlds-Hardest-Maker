using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

public partial class AnchorManager : MonoBehaviour
{
    public static AnchorManager Instance { get; private set; }
    
    private static readonly int selectedString = Animator.StringToHash("Selected");
    private static readonly int playingString = Animator.StringToHash("Playing");

    [SerializeField] [InitializationField] [MustBeAssigned] private AnchorParentController anchorPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform anchorContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private JumpToEntity mainCameraJumper;
    [SerializeField] [InitializationField] [MustBeAssigned] private ChainController mainChainController;
    [SerializeField] [InitializationField] [MustBeAssigned] private AnchorCameraJumping anchorCameraJumping;
    [SerializeField] [InitializationField] [MustBeAssigned] private AlphaTween anchorNoAnchorSelectedScreen;
    [SerializeField] [InitializationField] [MustBeAssigned] private AlphaTween anchorInPlayModeScreen;
    
    private EventBus eventBus;
    [Inject] private IAudioService audioService;
    [Inject] private ILevelObjectQuery<AnchorController> anchorQueryService;
    private AnchorFactory anchorFactory;
    
    [Inject]
    private void Construct(EventBus eventBus, AnchorFactory anchorFactory)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<StartPlaytestEvent>(OnPlaytest);
        eventBus.Subscribe<ResetLevelEvent>(OnResetLevel);
        eventBus.Subscribe<SetupPlaySceneEvent>(OnSetupPlayScene);

        this.anchorFactory = anchorFactory;
        anchorFactory.Initialize(anchorPrefab, anchorContainer);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        GameManager.Instance.DeselectInputs();
        UpdateBlockListInSelectedAnchor();
        anchorInPlayModeScreen.SetVisible(true);
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt) => anchorInPlayModeScreen.SetVisible(false);
    private void OnPlaytest(StartPlaytestEvent evt) => DeselectAnchor();
    private void OnResetLevel(ResetLevelEvent evt) => UpdateBlockListInSelectedAnchor();
    private void OnSetupPlayScene(SetupPlaySceneEvent evt) => UpdateBlockListInSelectedAnchor();
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<StartPlaytestEvent>(OnPlaytest);
        eventBus.Unsubscribe<ResetLevelEvent>(OnResetLevel);
        eventBus.Unsubscribe<SetupPlaySceneEvent>(OnSetupPlayScene);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    private void Update() => CheckAnchorSelection();
    
    /// <summary>
    ///     If anchor selected, convert anchor blocks in UI to <see cref="List{T}">List</see>&lt;<see cref="AnchorBlock" />&gt;
    ///     and apply it to selected anchor
    /// </summary>
    public void UpdateBlockListInSelectedAnchor()
    {
        if (SelectedAnchor == null) return;
        
        mainChainController.UpdateChildrenArray();
        List<AnchorBlock> blocksInChain = mainChainController.GetAnchorBlocks(SelectedAnchor);
        
        SelectedAnchor.Blocks = new(blocksInChain);
    }
    
    public void UpdateSelectedAnchorLines()
    {
        if (SelectedAnchor == null) return;
        
        // update list of blocks in anchor
        UpdateBlockListInSelectedAnchor();
        
        SelectedAnchor.RenderLines();
    }
}