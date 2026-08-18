using MyBox;
using UnityEngine;
using VContainer;

public class PanelVisibilityCoordinator : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController levelSettingsPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController testingOptionsPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorAttachButtonController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorAttachExitButtonController;
    
    private bool wasAnchorPanelOpen;

    [Inject] private EventBus eventBus;
    [Inject] private IPanelService panelService;
    [Inject] private IPanelRegistry panelRegistry; // need later for loops and such
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        // hide all panels
        panelService.SetPanelHidden(levelSettingsPanelController, true);
        
        panelService.SetPanelHidden(testingOptionsPanelController, true);
        
        wasAnchorPanelOpen = anchorPanelController.Open;
        panelService.SetPanelHidden(anchorPanelController, true);
        panelService.SetPanelHidden(anchorAttachButtonController, true);
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt)
    {
        // show level setting / anchor panel
        bool isEditModeAnchorRelated = LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated;
        if (isEditModeAnchorRelated)
        {
            if (wasAnchorPanelOpen) panelService.SetPanelOpen(anchorPanelController, true);
            else panelService.SetPanelHidden(anchorPanelController, false);
            
            if (AnchorManager.Instance.SelectedAnchor != null) panelService.SetPanelHidden(anchorAttachButtonController, false, false);
        }
        else
        {
            panelService.SetPanelHidden(levelSettingsPanelController, false, false);
            panelService.SetPanelHidden(testingOptionsPanelController, false, false);
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