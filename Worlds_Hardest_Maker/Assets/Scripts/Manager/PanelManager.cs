using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

/// <summary>
/// DIESER CODE IST SO SCHLECHT HOLY SHIT ES IST ALLES SO UNÜBERSICHTLICH ZEIGE DEINEM ARBEITGEBER NIEMALS DIESEN CODE
/// </summary>
public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }
    
    [ReadOnly] public List<PanelController> Panels;
    
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController levelSettingsPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController testingOptionsPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorAttachButtonController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorAttachExitButtonController;
    
    public bool WasAnchorPanelOpen { get; set; }
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
    
    public void SetPanelOpen(PanelController panel, bool open, bool hideOtherPanels = true)
    {
        panel.SetOpen(open);
        
        // if opening panel, hide every other panel
        if (!open) return;
        
        if (!hideOtherPanels) return;
        
        foreach (PanelController panelController in Panels)
        {
            if (panelController == panel) continue;
            
            panelController.SetHidden(true);
        }
    }
    
    public void SetPanelHidden(PanelController panel, bool hidden, bool hideOtherPanels = true)
    {
        panel.SetHidden(hidden);
        
        // if showing panel, hide every other panel
        if (hidden) return;
        
        if (!hideOtherPanels) return;
        
        foreach (PanelController panelController in Panels)
        {
            if (panelController == panel) continue;
            
            panelController.SetHidden(true);
        }
    }
    
    public void CloseAllPanels()
    {
        foreach (PanelController panel in Panels) SetPanelOpen(panel, false);
    }
    
    public void HideAllPanels()
    {
        foreach (PanelController panel in Panels) SetPanelHidden(panel, true);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        // hide all panels
        SetPanelHidden(levelSettingsPanelController, true);
        
        SetPanelHidden(testingOptionsPanelController, true);
        
        WasAnchorPanelOpen = anchorPanelController.Open;
        SetPanelHidden(anchorPanelController, true);
        SetPanelHidden(anchorAttachButtonController, true);
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt)
    {
        // show level setting / anchor panel
        bool isEditModeAnchorRelated = LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated;
        if (isEditModeAnchorRelated)
        {
            if (WasAnchorPanelOpen) SetPanelOpen(anchorPanelController, true);
            else SetPanelHidden(anchorPanelController, false);
            
            if (AnchorManager.Instance.SelectedAnchor != null) SetPanelHidden(anchorAttachButtonController, false, false);
        }
        else
        {
            SetPanelHidden(levelSettingsPanelController, false, false);
            SetPanelHidden(testingOptionsPanelController, false, false);
        }
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
}