using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(PanelTween))]
public class PanelController : MonoBehaviour, IPanel
{
    [SerializeField] [InitializationField] [CanBeNull] private PanelTween panelTween;
    private bool hasPanelTween;
    
    [SerializeField] [InitializationField] [CanBeNull] private PanelTween buttonPanelTween;
    private bool hasButtonPanelTween;

    [field: Separator("Initial settings")]
    [field: SerializeField]
    [field: InitializationField]
    public bool Open { get; private set; }

    [field: SerializeField]
    [field: InitializationField]
    public bool Hidden { get; private set; }

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
        Open = open;
        
        if (hasPanelTween) panelTween.SetOpen(Open);
        
        // un-hide panel if hidden
        if (Open && Hidden) SetHidden(false);
    }
    
    public void SetHidden(bool hidden)
    {
        // hide/show button
        Hidden = hidden;
        
        // close panel if open
        if (Hidden && Open) SetOpen(false);
        
        if (hasButtonPanelTween) buttonPanelTween.SetOpen(!Hidden);
    }
    
    private void Start()
    {
        hasPanelTween = panelTween != null;
        hasButtonPanelTween = buttonPanelTween != null;
        
        if (hasPanelTween) panelTween.SetOpen(Open, true);
        if (hasButtonPanelTween) buttonPanelTween.SetOpen(!Hidden, true);
    }
    
    public void OnButtonToggle()
    {
        if (hasButtonPanelTween && !buttonPanelTween.Open) return;
        
        panelService.SetPanelOpen(this, !Open);
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