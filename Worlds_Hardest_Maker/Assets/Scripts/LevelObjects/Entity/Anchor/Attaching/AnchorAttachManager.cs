using MyBox;
using UnityEngine;
using VContainer;

public partial class AnchorAttachManager : MonoBehaviour
{
    public static AnchorAttachManager Instance { get; private set; }
    
    [ReadOnly] public bool InAttachMode;
    
    private static readonly int editingString = Animator.StringToHash("Editing");
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        if (InAttachMode) ExitAttachMode();
    }
    
    public void EnterAttachMode()
    {
        if (LevelSessionEditManager.Instance.IsPlaying
            || !LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated
            || AnchorManager.Instance.SelectedAnchor == null
            || AnchorPositionInputEditManager.Instance.IsEditing) return;
        
        InAttachMode = true;
        
        LevelSessionEditManager.Instance.SetEditMode(EditModeManager.Ball);
        
        HighlightAnchor(AnchorManager.Instance.SelectedAnchor);
        
        eventBus.Fire(new EnterAnchorAttachEvent());
    }
    
    public void ExitAttachMode()
    {
        InAttachMode = false;
        
        if (AnchorManager.Instance.SelectedAnchor)
        {
            bool isModeAnchorRelated = LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated;
            AnchorManager.Instance.SelectedAnchor.GetComponent<Animator>().SetBool(editingString, isModeAnchorRelated);
            AnchorManager.Instance.SelectedAnchor.SetLinesActive(isModeAnchorRelated);
        }
        
        Dehighlight(AnchorManager.Instance.SelectedAnchor);
        
        eventBus.Fire(new ExitAnchorAttachEvent());
    }
    
    public static Transform GetCurrentAnchorContainer() => Instance.InAttachMode ? AnchorManager.Instance.SelectedAnchor.AttachmentContainer : null;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
    }
}