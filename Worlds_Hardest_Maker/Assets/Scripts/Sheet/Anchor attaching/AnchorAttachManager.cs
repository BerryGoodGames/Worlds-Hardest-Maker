using MyBox;
using UnityEngine;
using VContainer;

public partial class AnchorAttachManager : MonoBehaviour
{
    public static AnchorAttachManager Instance { get; private set; }
    
    [ReadOnly] public bool InAttachMode;
    
    private EventBus eventBus;
    [Inject] private IAnchorManager anchorManager;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<AnchorDeselectedEvent>(OnAnchorDeselected);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        if (InAttachMode) ExitAttachMode();
    }

    private void OnAnchorDeselected(AnchorDeselectedEvent evt)
    {
        if (InAttachMode) ExitAttachMode();
    }

    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<AnchorDeselectedEvent>(OnAnchorDeselected);
    }
    
    public void EnterAttachMode()
    {
        if (LevelSessionEditManager.Instance.IsPlaying
            || anchorManager.SelectedAnchor == null
            || AnchorPositionInputEditManager.Instance.IsEditing) return;
        
        InAttachMode = true;
        
        HighlightAnchor(anchorManager.SelectedAnchor);
        
        eventBus.Fire(new EnterAnchorAttachEvent());
    }
    
    public void ExitAttachMode()
    {
        InAttachMode = false;
        
        Dehighlight(anchorManager.SelectedAnchor);
        
        eventBus.Fire(new ExitAnchorAttachEvent());
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}