using System.Collections.Generic;
using UnityEngine;
using Zenject;

public partial class AnchorManager : MonoBehaviour
{
    public static AnchorManager Instance { get; private set; }
    
    private static readonly int selectedString = Animator.StringToHash("Selected");
    private static readonly int playingString = Animator.StringToHash("Playing");
    
    private DiContainer diContainer;
    
    private EventBus eventBus;
    
    private IAudioService audioService;
    
    [Inject]
    private void Construct(DiContainer diContainer, EventBus eventBus, IAudioService audioService)
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
        GameManager.DeselectInputs();
        UpdateBlockListInSelectedAnchor();
        StartExecuting();
        ReferenceManager.Instance.AnchorInPlayModeScreen.SetVisible(true);
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt)
    {
        ReferenceManager.Instance.AnchorInPlayModeScreen.SetVisible(false);
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
        
        ReferenceManager.Instance.MainChainController.UpdateChildrenArray();
        List<AnchorBlock> blocksInChain = ReferenceManager.Instance.MainChainController.GetAnchorBlocks(SelectedAnchor);
        
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
        foreach (Transform t in ReferenceManager.Instance.AnchorContainer)
        {
            AnchorParentController parent = t.GetComponent<AnchorParentController>();
            AnchorController anchor = parent.Child;
            
            anchor.StartExecuting();
            
            anchor.SetLinesActive(false);
            
            if (SelectedAnchor == anchor &&
                LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated) continue;
            
            anchor.Animator.SetBool(playingString, true);
        }
    }
}