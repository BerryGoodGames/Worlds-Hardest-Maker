using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

[RequireComponent(typeof(PanelTween))]
public class PanelController : MonoBehaviour, IPanel
{ 
    private PanelTween panelTween;

    [SerializeField] [InitializationField] [MustBeAssigned]
    private PanelTween buttonPanelTween;

    [field: FormerlySerializedAs("<Open>k__BackingField")]
    [field: Separator("Initial settings")]
    [field: SerializeField]
    [field: InitializationField]
    public bool IsOpen { get; private set; }

    [field: FormerlySerializedAs("<Hidden>k__BackingField")]
    [field: SerializeField]
    [field: InitializationField]
    public bool IsHidden { get; private set; }

    [field: SerializeField]
    [field: InitializationField]
    public bool CloseOnEscape { get; private set; }

    [field: SerializeField]
    [field: InitializationField]
    public PanelExclusionGroup ExclusionGroup { get; private set; } = PanelExclusionGroup.EditMain;

    [Inject] private IPanelRegistry panelRegistry;
    [Inject] private IPanelService panelService;

    private void Awake()
    {
        panelTween = GetComponent<PanelTween>();
    }

    public void SetOpen(bool open)
    {
        // open/close panel
        IsOpen = open;
        panelTween.SetOpen(IsOpen);
        
        // un-hide panel if hidden
        if (IsOpen && IsHidden) SetHidden(false);
    }
    
    public void SetHidden(bool hidden)
    {
        // hide/show button
        IsHidden = hidden;
        buttonPanelTween.SetOpen(!IsHidden);
        
        // close panel if open
        if (IsHidden && IsOpen) SetOpen(false);
    }
    
    public void OnButtonToggle()
    {
        if (buttonPanelTween.Open)
        {
            panelService.SetPanelOpen(this, !IsOpen);
        }
    }

    private void OnEnable()
    {
        panelRegistry.Register(this);
        panelTween.SetOpen(IsOpen, true);
        buttonPanelTween.SetOpen(!IsHidden, true);
    }

    private void OnDisable()
    {
        panelRegistry.Unregister(this);
    }
}