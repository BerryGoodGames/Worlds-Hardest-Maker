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
    
    [field: Separator("Initial settings")] [field: SerializeField] [field: InitializationField] public bool Open { get; private set; }
    
    [field: SerializeField] [field: InitializationField] public bool Hidden { get; private set; }
    
    [field: SerializeField] [field: InitializationField] public bool CloseOnEscape { get; private set; }

    [Inject] private IPanelRegistry panelRegistry;
    [Inject] private IPanelService panelService;
    
    public void ToggleOpen(bool noAnimation = false) => SetOpen(!Open, noAnimation);
    
    public void SetOpen(bool open, bool noAnimation = false)
    {
        // open/close panel
        Open = open;
        
        if (hasPanelTween) panelTween.SetOpen(Open, noAnimation);
        
        // un-hide panel if hidden
        if (Open && Hidden) SetHidden(false, noAnimation);
    }
    
    public void ToggleHidden(bool noAnimation = false) => SetHidden(!Hidden, noAnimation);
    
    public void SetHidden(bool hidden, bool noAnimation = false)
    {
        // hide/show button
        Hidden = hidden;
        
        // close panel if open
        if (Hidden && Open) SetOpen(false, noAnimation);
        
        if (hasButtonPanelTween) buttonPanelTween.SetOpen(!Hidden, noAnimation);
    }
    
    private void Start()
    {
        hasPanelTween = panelTween != null;
        hasButtonPanelTween = buttonPanelTween != null;
        
        if (hasPanelTween) panelTween.SetOpen(Open, true);
        if (hasButtonPanelTween) buttonPanelTween.SetOpen(!Hidden, true);
    }
    
    public void OnButtonToggle(bool hideOtherPanels = true)
    {
        if (hasButtonPanelTween && !buttonPanelTween.Open) return;
        
        panelService.SetPanelOpen(this, !Open, hideOtherPanels);
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