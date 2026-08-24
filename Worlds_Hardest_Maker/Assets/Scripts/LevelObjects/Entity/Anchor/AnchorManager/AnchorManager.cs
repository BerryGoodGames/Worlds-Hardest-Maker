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
    
    private IObjectResolver diContainer;
    private EventBus eventBus;
    private IAudioService audioService;
    
    [Inject]
    private void Construct(IObjectResolver diContainer, EventBus eventBus, IAudioService audioService)
    {
        this.diContainer = diContainer;
        this.eventBus = eventBus;
        this.audioService = audioService;
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<StartPlaytestEvent>(OnPlaytest);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        GameManager.Instance.DeselectInputs();
        UpdateBlockListInSelectedAnchor();
        StartExecuting();
        anchorInPlayModeScreen.SetVisible(true);
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt)
    {
        anchorInPlayModeScreen.SetVisible(false);
    }
    
    private void OnPlaytest(StartPlaytestEvent evt)
    {
        DeselectAnchor();
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
        AnchorController selectedAnchor = Instance.SelectedAnchor;
        if (selectedAnchor == null) return;
        
        // update list of blocks in anchor
        Instance.UpdateBlockListInSelectedAnchor();
        
        selectedAnchor.RenderLines();
    }
    
    public void StartExecuting()
    {
        UpdateBlockListInSelectedAnchor();
        
        // let anchors start executing
        foreach (Transform t in anchorContainer)
        {
            AnchorParentController parent = t.GetComponent<AnchorParentController>();
            AnchorController anchor = parent.Child;
            
            anchor.StartExecuting();
            
            anchor.SetLinesActive(false);
            
            if (SelectedAnchor == anchor &&
                LevelSessionEditManager.Instance.CurrentEditMode.IsAnchorRelated) continue;
            
            anchor.Animator.SetBool(playingString, true);
        }
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<StartPlaytestEvent>(OnPlaytest);
    }
}