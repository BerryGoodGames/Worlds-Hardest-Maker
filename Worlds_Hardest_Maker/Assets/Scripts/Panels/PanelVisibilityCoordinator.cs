using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public class PanelVisibilityCoordinator : MonoBehaviour
{
    [SerializeField] private List<PanelVisibilityRule> rules;

    private PanelUIState? currentState;
    
    [Inject] private EventBus eventBus;
    [Inject] private IPanelService panelService;
    [Inject] private IPanelRegistry panelRegistry; // need later for loops and such
    
    private void ApplyState(PanelUIState newState)
    {
        IReadOnlyCollection<IPanel> oldVisiblePanels = currentState.HasValue
            ? GetVisiblePanels(currentState.Value)
            : new List<IPanel>();

        IReadOnlyCollection<IPanel> newVisiblePanels = GetVisiblePanels(newState);

        foreach (IPanel panel in oldVisiblePanels)
        {
            if (!newVisiblePanels.Contains(panel))
            {
                panelService.SetPanelHidden(panel, true);
            }
        }

        foreach (IPanel panel in newVisiblePanels)
        {
            if (!oldVisiblePanels.Contains(panel))
            {
                panelService.SetPanelHidden(panel, false);
            }
        }

        currentState = newState;
    }
    
    private IReadOnlyCollection<IPanel> GetVisiblePanels(PanelUIState? state)
    {
        if (state == null)
        {
            return new List<IPanel>();
        }

        PanelVisibilityRule rule = rules.Find(r => r.UIState == state.Value);

        if (rule == null)
        {
            return new List<IPanel>();
        }

        return rule.VisiblePanels
            .Cast<IPanel>()
            .ToList();
    }

    private void RestoreCurrentState()
    {
        if (!currentState.HasValue) return;

        IReadOnlyCollection<IPanel> visiblePanels = GetVisiblePanels(currentState.Value);

        foreach (IPanel panel in panelRegistry.RegisteredPanels)
        {
            panelService.SetPanelHidden(panel, !visiblePanels.Contains(panel));
        }
    }

    private void OnPanelClosed(PanelClosedEvent evt)
    {
        RestoreCurrentState();
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => ApplyState(PanelUIState.Playing);
    private void OnSwitchToEdit(SwitchToEditEvent evt) => ApplyState(CurrentEditState());
    private void OnEditModeInitialized(EditModeInitializedEvent evt) => ApplyState(CurrentEditState());
    private void OnEditModeChange(EditModeChangeEvent evt) => ApplyState(CurrentEditState());
    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt) => ApplyState(PanelUIState.EditingAnchorAttach);
    private void OnExitAnchorAttach(ExitAnchorAttachEvent evt) => ApplyState(CurrentEditState());
    private void OnAnchorSelected(AnchorSelectedEvent evt) => ApplyState(PanelUIState.EditingAnchor);
    private void OnAnchorDeselected(AnchorDeselectedEvent evt) => ApplyState(CurrentEditState());
    private void OnAnchorPositionEditStarted(AnchorPositionEditStartedEvent evt) => ApplyState(PanelUIState.EditingAnchorPositionInputEdit);
    private void OnAnchorPositionEditEnded(AnchorPositionEditEndedEvent evt) => ApplyState(CurrentEditState());

    private PanelUIState CurrentEditState()
    {
        bool isAnchorRelated = LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated;
        return isAnchorRelated ? PanelUIState.EditingAnchor : PanelUIState.EditingGeneral;
    }
    
    // private void OnSwitchToPlay(SwitchToPlayEvent evt)
    // {
    //     // hide all panels
    //     panelService.SetPanelHidden(levelSettingsPanelController, true, true);
    //     
    //     panelService.SetPanelHidden(testingOptionsPanelController, true, true);
    //     
    //     wasAnchorPanelOpen = anchorPanelController.Open;
    //     panelService.SetPanelHidden(anchorPanelController, true, true);
    //     panelService.SetPanelHidden(anchorAttachButtonController, true, true);
    // }
    //
    // private void OnSwitchToEdit(SwitchToEditEvent evt)
    // {
    //     // show level setting / anchor panel
    //     bool isEditModeAnchorRelated = LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated;
    //     if (isEditModeAnchorRelated)
    //     {
    //         if (wasAnchorPanelOpen) panelService.SetPanelOpen(anchorPanelController, true, true);
    //         else panelService.SetPanelHidden(anchorPanelController, false, true);
    //         
    //         if (AnchorManager.Instance.SelectedAnchor != null) panelService.SetPanelHidden(anchorAttachButtonController, false, false);
    //     }
    //     else
    //     {
    //         panelService.SetPanelHidden(levelSettingsPanelController, false, false);
    //         panelService.SetPanelHidden(testingOptionsPanelController, false, false);
    //     }
    // }
    //
    // private void OnEditModeChange(EditModeChangeEvent evt)
    // {
    //     // open corresponding panel
    //     if (!AnchorAttachManager.Instance.InAttachMode)
    //     {
    //         if (evt.NewEditMode.Attributes.IsAnchorRelated)
    //         {
    //             panelService.SetPanelHidden(anchorPanelController, false, true);
    //
    //             if (AnchorManager.Instance.SelectedAnchor)
    //             {
    //                 panelService.SetPanelHidden(anchorAttachButtonController, false, false);
    //             }
    //         }
    //         else
    //         {
    //             panelService.SetPanelHidden(levelSettingsPanelController, false, false);
    //             panelService.SetPanelHidden(testingOptionsPanelController, false, false);
    //         }
    //     }
    // }
    //
    // private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt)
    // {
    //     panelService.SetPanelHidden(anchorAttachButtonController, true, true);
    //     panelService.SetPanelHidden(anchorAttachExitButtonController, false, false);
    //     
    //     panelService.SetPanelOpen(anchorPanelController, false, false);
    // }
    //
    // private void OnExitAnchorAttach(ExitAnchorAttachEvent evt)
    // {
    //     if (LevelSessionEditManager.Instance.IsEditing)
    //     {
    //         panelService.SetPanelHidden(anchorAttachButtonController, false, false);
    //     }
    //     
    //     panelService.SetPanelHidden(anchorAttachExitButtonController, true, true);
    //     if (!LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated)
    //     {
    //         panelService.SetPanelHidden(levelSettingsPanelController, false, false);
    //         panelService.SetPanelHidden(testingOptionsPanelController, false, false);
    //     }
    // }
    //
    // private void OnAnchorSelected(AnchorSelectedEvent evt)
    // {
    //     if (!AnchorAttachManager.Instance.InAttachMode)
    //     {
    //         panelService.SetPanelHidden(anchorAttachButtonController, false, false);
    //     }
    // }
    //
    // private void OnAnchorDeselected(AnchorDeselectedEvent evt)
    // {
    //     EditMode currentEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
    //     if (!currentEditMode.Attributes.IsAnchorRelated)
    //     {
    //         panelService.SetPanelHidden(levelSettingsPanelController, false, false);
    //         panelService.SetPanelHidden(testingOptionsPanelController, false, false);
    //     }
    //     
    //     panelService.SetPanelHidden(anchorAttachButtonController, true, true);
    //     panelService.SetPanelHidden(anchorAttachExitButtonController, true, true);
    // }
    //
    // private void OnAnchorPositionEditStarted(AnchorPositionEditStartedEvent evt)
    // {
    //     panelService.SetPanelHidden(anchorPanelController, true, true);
    //     panelService.SetPanelHidden(anchorAttachButtonController, true, true);
    //     panelService.SetPanelHidden(anchorAttachExitButtonController, true, true);
    // }
    //
    // private void OnAnchorPositionEditEnded(AnchorPositionEditEndedEvent evt)
    // {
    //     bool isEditing = LevelSessionEditManager.Instance.IsEditing;
    //     bool isAttaching = AnchorAttachManager.Instance.InAttachMode;
    //     
    //     panelService.SetPanelOpen(anchorPanelController, isEditing, true);
    //     panelService.SetPanelOpen(anchorAttachButtonController, isEditing && !isAttaching, false);
    //     panelService.SetPanelOpen(anchorAttachExitButtonController, isEditing && isAttaching, false);
    // }
    
    private void OnEnable()
    {
        eventBus.Subscribe<PanelClosedEvent>(OnPanelClosed);
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<EditModeInitializedEvent>(OnEditModeInitialized);
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
        eventBus.Unsubscribe<PanelClosedEvent>(OnPanelClosed);
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<EditModeInitializedEvent>(OnEditModeInitialized);
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Unsubscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Unsubscribe<ExitAnchorAttachEvent>(OnExitAnchorAttach);
        eventBus.Unsubscribe<AnchorSelectedEvent>(OnAnchorSelected);
        eventBus.Unsubscribe<AnchorDeselectedEvent>(OnAnchorDeselected);
        eventBus.Unsubscribe<AnchorPositionEditStartedEvent>(OnAnchorPositionEditStarted);
        eventBus.Unsubscribe<AnchorPositionEditEndedEvent>(OnAnchorPositionEditEnded);
    }
}