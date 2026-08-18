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

    private void OnEditModeChange(EditModeChangeEvent evt)
    {
        // open corresponding panel
        if (!AnchorAttachManager.Instance.InAttachMode)
        {
            if (evt.NewEditMode.Attributes.IsAnchorRelated)
            {
                panelService.SetPanelHidden(anchorPanelController, false);

                if (AnchorManager.Instance.SelectedAnchor)
                {
                    panelService.SetPanelHidden(anchorAttachButtonController, false, false);
                }
            }
            else
            {
                panelService.SetPanelHidden(levelSettingsPanelController, false, false);
                panelService.SetPanelHidden(testingOptionsPanelController, false, false);
            }
        }
    }

    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt)
    {
        panelService.SetPanelHidden(anchorAttachButtonController, true);
        panelService.SetPanelHidden(anchorAttachExitButtonController, false, false);
        
        panelService.SetPanelOpen(anchorPanelController, false, false);
    }

    private void OnExitAnchorAttach(ExitAnchorAttachEvent evt)
    {
        if (LevelSessionEditManager.Instance.IsEditing)
        {
            panelService.SetPanelHidden(anchorAttachButtonController, false, false);
        }
        
        panelService.SetPanelHidden(anchorAttachExitButtonController, true);
        if (!LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated)
        {
            panelService.SetPanelHidden(levelSettingsPanelController, false, false);
            panelService.SetPanelHidden(testingOptionsPanelController, false, false);
        }
    }

    private void OnAnchorSelected(AnchorSelectedEvent evt)
    {
        if (!AnchorAttachManager.Instance.InAttachMode)
        {
            panelService.SetPanelHidden(anchorAttachButtonController, false, false);
        }
    }

    private void OnAnchorDeselected(AnchorDeselectedEvent evt)
    {
        EditMode currentEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
        if (!currentEditMode.Attributes.IsAnchorRelated)
        {
            panelService.SetPanelHidden(levelSettingsPanelController, false, false);
            panelService.SetPanelHidden(testingOptionsPanelController, false, false);
        }
        
        panelService.SetPanelHidden(anchorAttachButtonController, true);
        panelService.SetPanelHidden(anchorAttachExitButtonController, true);
    }

    private void OnAnchorPositionEditStarted(AnchorPositionEditStartedEvent evt)
    {
        panelService.SetPanelHidden(anchorPanelController, true);
        panelService.SetPanelHidden(anchorAttachButtonController, true);
        panelService.SetPanelHidden(anchorAttachExitButtonController, true);
    }

    private void OnAnchorPositionEditEnded(AnchorPositionEditEndedEvent evt)
    {
        bool isEditing = LevelSessionEditManager.Instance.IsEditing;
        bool isAttaching = AnchorAttachManager.Instance.InAttachMode;
        
        panelService.SetPanelOpen(anchorPanelController, isEditing);
        panelService.SetPanelOpen(anchorAttachButtonController, isEditing && !isAttaching, false);
        panelService.SetPanelOpen(anchorAttachExitButtonController, isEditing && isAttaching, false);
    }
    
    private void OnEnable()
    {
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Subscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Subscribe<ExitAnchorAttachEvent>(OnExitAnchorAttach);
        eventBus.Subscribe<AnchorSelectedEvent>(OnAnchorSelected);
        eventBus.Subscribe<AnchorDeselectedEvent>(OnAnchorDeselected);
        eventBus.Subscribe<AnchorPositionEditStartedEvent>(OnAnchorPositionEditStarted);
        eventBus.Subscribe<AnchorPositionEditEndedEvent>(OnAnchorPositionEditEnded);
    }

    private void OnDisable()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Unsubscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Unsubscribe<ExitAnchorAttachEvent>(OnExitAnchorAttach);
        eventBus.Unsubscribe<AnchorSelectedEvent>(OnAnchorSelected);
        eventBus.Unsubscribe<AnchorDeselectedEvent>(OnAnchorDeselected);
        eventBus.Unsubscribe<AnchorPositionEditStartedEvent>(OnAnchorPositionEditStarted);
        eventBus.Unsubscribe<AnchorPositionEditEndedEvent>(OnAnchorPositionEditEnded);
    }
}