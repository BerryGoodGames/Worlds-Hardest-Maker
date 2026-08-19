using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public class PanelVisibilityCoordinator : MonoBehaviour
{
    [SerializeField] private List<PanelVisibilityRule> rules;

    private PanelUIState currentState;
    private readonly PanelUIStateService stateService = new();
    
    [Inject] private EventBus eventBus;
    [Inject] private IPanelService panelService;
    [Inject] private IPanelRegistry panelRegistry;
    
    private void ApplyCurrentState()
    {
        PanelUIState newState = stateService.GetCurrentUIState();
        
        IReadOnlyCollection<IPanel> oldVisiblePanels = GetVisiblePanels(currentState);
        
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
        IReadOnlyCollection<IPanel> visiblePanels = GetVisiblePanels(currentState);

        foreach (IPanel panel in panelRegistry.RegisteredPanels)
        {
            panelService.SetPanelHidden(panel, !visiblePanels.Contains(panel));
        }
    }

    private void OnPanelClosed(PanelClosedEvent evt)
    {
        RestoreCurrentState();
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => ApplyCurrentState();
    private void OnSwitchToEdit(SwitchToEditEvent evt) => ApplyCurrentState();
    private void OnEditModeInitialized(EditModeInitializedEvent evt) => ApplyCurrentState();
    private void OnEditModeChange(EditModeChangeEvent evt) => ApplyCurrentState();
    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt) => ApplyCurrentState();
    private void OnExitAnchorAttach(ExitAnchorAttachEvent evt) => ApplyCurrentState();
    private void OnAnchorSelected(AnchorSelectedEvent evt) => ApplyCurrentState();
    private void OnAnchorDeselected(AnchorDeselectedEvent evt) => ApplyCurrentState();
    private void OnAnchorPositionEditStarted(AnchorPositionEditStartedEvent evt) => ApplyCurrentState();
    private void OnAnchorPositionEditEnded(AnchorPositionEditEndedEvent evt) => ApplyCurrentState();
    
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