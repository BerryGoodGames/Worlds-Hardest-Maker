using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

[RequireComponent(typeof(PanelTween))] // TODO: contradiction
public class PanelController : MonoBehaviour, IPanel
{
    [SerializeField] [InitializationField] [CanBeNull] private PanelTween panelTween;
    private bool hasPanelTween;
    
    // TODO: separate button animations from panel animations
    [SerializeField] [InitializationField] [CanBeNull] private PanelTween buttonPanelTween;
    private bool hasButtonPanelTween;

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
    
    public void SetOpen(bool open)
    {
        // open/close panel
        IsOpen = open;
        
        if (hasPanelTween) panelTween.SetOpen(IsOpen);
        
        // un-hide panel if hidden
        if (IsOpen && IsHidden) SetHidden(false);
    }
    
    public void SetHidden(bool hidden)
    {
        // hide/show button
        IsHidden = hidden;
        
        // close panel if open
        if (IsHidden && IsOpen) SetOpen(false);
        
        if (hasButtonPanelTween) buttonPanelTween.SetOpen(!IsHidden);
    }
    
    private void Start()
    {
        hasPanelTween = panelTween != null;
        hasButtonPanelTween = buttonPanelTween != null;
        
        if (hasPanelTween) panelTween.SetOpen(IsOpen, true);
        if (hasButtonPanelTween) buttonPanelTween.SetOpen(!IsHidden, true);
    }
    
    public void OnButtonToggle()
    {
        if (hasButtonPanelTween && !buttonPanelTween.Open) return;
        
        panelService.SetPanelOpen(this, !IsOpen);
    }

    private void OnEnable()
    {
        panelRegistry.Register(this);
    }

    private void OnDisable()
    {
        panelRegistry.Unregister(this);
    }
}