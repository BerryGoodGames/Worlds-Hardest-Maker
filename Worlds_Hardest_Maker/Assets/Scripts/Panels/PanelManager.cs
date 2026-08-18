using MyBox;
using UnityEngine;
using VContainer;

/// <summary>
/// DIESER CODE IST SO SCHLECHT HOLY SHIT ES IST ALLES SO UNÜBERSICHTLICH ZEIGE DEINEM ARBEITGEBER NIEMALS DIESEN CODE
/// </summary>
public class PanelManager : MonoBehaviour, IPanelService
{
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController levelSettingsPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController testingOptionsPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorAttachButtonController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorAttachExitButtonController;

    private bool wasAnchorPanelOpen;
    
    [Inject] private EventBus eventBus;
    [Inject] private IPanelRegistry panelRegistry;
    
    public void SetPanelOpen(IPanel panel, bool open, bool hideOtherPanels = true)
    {
        panel.SetOpen(open);
        
        // if opening panel, hide every other panel
        if (!open) return;
        
        if (!hideOtherPanels) return;
        
        foreach (IPanel otherPanel in panelRegistry.RegisteredPanels)
        {
            if (otherPanel == panel) continue;
            
            otherPanel.SetHidden(true);
        }
    }
    
    public void SetPanelHidden(IPanel panel, bool hidden, bool hideOtherPanels = true)
    {
        panel.SetHidden(hidden);
        
        // if showing panel, hide every other panel
        if (hidden) return;
        
        if (!hideOtherPanels) return;
        
        foreach (IPanel otherPanel in panelRegistry.RegisteredPanels)
        {
            if (otherPanel == panel) continue;
            
            otherPanel.SetHidden(true);
        }
    }
    
    public void CloseAllPanels()
    {
        foreach (IPanel panel in panelRegistry.RegisteredPanels) SetPanelOpen(panel, false);
    }
    
    public void HideAllPanels()
    {
        foreach (IPanel panel in panelRegistry.RegisteredPanels) SetPanelHidden(panel, true);
    }

    public bool TryCloseOnEscape()
    {
        bool closingPanel = false;
        
        if (!Input.GetKeyDown(KeyCode.Escape)) return false;
        
        foreach (IPanel panel in panelRegistry.RegisteredPanels)
        {
            if (!panel.Open || !panel.CloseOnEscape) continue;
            
            SetPanelOpen(panel, false);
            closingPanel = true;
        }
        
        return closingPanel;
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        // hide all panels
        SetPanelHidden(levelSettingsPanelController, true);
        
        SetPanelHidden(testingOptionsPanelController, true);
        
        wasAnchorPanelOpen = anchorPanelController.Open;
        SetPanelHidden(anchorPanelController, true);
        SetPanelHidden(anchorAttachButtonController, true);
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt)
    {
        // show level setting / anchor panel
        bool isEditModeAnchorRelated = LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated;
        if (isEditModeAnchorRelated)
        {
            if (wasAnchorPanelOpen) SetPanelOpen(anchorPanelController, true);
            else SetPanelHidden(anchorPanelController, false);
            
            if (AnchorManager.Instance.SelectedAnchor != null) SetPanelHidden(anchorAttachButtonController, false, false);
        }
        else
        {
            SetPanelHidden(levelSettingsPanelController, false, false);
            SetPanelHidden(testingOptionsPanelController, false, false);
        }
    }

    private void OnEnable()
    {
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }

    private void OnDisable()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
}